using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Client network manager.
/// </summary>
public class ClientNetworkManager : ExtendedMonoBehaviour
{
    const int Port = 7788;
    const float ReceivePermissionToConnectTimeoutInSeconds = 3f;
    const float SendKeepAliveRequestDelayInSeconds = 1f;

    public NotificationsServiceController NotificationsServiceController;
    //how many times to try to connect to server before disconnecting and start searching for another
    public byte RetriesBeforeSearchingForAnotherServer = 3;

    public EventHandler OnConnectedEvent = delegate
    {
    };

    public EventHandler<DataSentEventArgs> OnReceivedDataEvent = delegate
    {
    };

    public EventHandler OnDisconnectedEvent = delegate
    {
    };


    int connectionId = 0;
    int genericHostId = 0;

    ConnectionConfig connectionConfig = null;
    byte communicationChannel = 0;

    bool isConnected = false;
    bool isRunning = false;

    Dictionary<string, Action<NetworkData>> commands = new Dictionary<string, Action<NetworkData>>();

    public bool IsConnected
    {
        get
        {
            return isConnected;
        }
    }

    void Start()
    {
        ConfigureCommands();
        ConfigureClient();

        CoroutineUtils.RepeatEverySeconds(SendKeepAliveRequestDelayInSeconds, SendKeepAliveRequest);

        StartCoroutine(UpdateCoroutine());
    }

    void ConfigureClient()
    {
        connectionConfig = new ConnectionConfig();
        connectionConfig.MaxConnectionAttempt = RetriesBeforeSearchingForAnotherServer; 
        communicationChannel = connectionConfig.AddChannel(QosType.ReliableSequenced); //make sure messages are delivered and send in correct order
    }

    void ConfigureCommands()
    {
        commands["KickReason"] = KickedFromServer;
        commands["AllowedToConnect"] = AllowedToConnectToServer;
    }

    void SendKeepAliveRequest()
    {
        if (isRunning && isConnected)
        {
            SendServerMessage("KeepAlive");
        }
    }

    void ShowNotification(Color color, string message)
    {
        if (NotificationsServiceController != null)
        {
            NotificationsServiceController.AddNotification(color, message);          
        }
    }

    IEnumerator UpdateCoroutine()
    {
        while (true)
        {
            if (isRunning)
            {
                UpdateClient();
            }

            yield return new WaitForSeconds(0.5f);
        }
    }

    string GetUsername()
    {
        var username = "";

        if (PlayerPrefs.HasKey("Username"))
        {
            username = PlayerPrefs.GetString("Username");
        }

        return username;
    }

    void UpdateClient()
    {
        if (!isRunning)
        {
            return;
        }

        NetworkData receiveNetworkData = new NetworkData(0, null, NetworkEventType.Nothing);

        try
        {
            receiveNetworkData = NetworkTransportUtils.ReceiveMessage();
        }
        catch (NetworkException e)
        {
            var error = (NetworkError)e.ErrorN;
            var errorMessage = NetworkErrorUtils.GetMessage(error);

            NotificationsServiceController.AddNotification(Color.red, errorMessage);

            //if cannot connect to server
            if (error == NetworkError.Timeout)
            {
                //disconnect
                Disconnect();
            }

            return;
        }

        switch (receiveNetworkData.NetworkEventType)
        {
            case NetworkEventType.DataEvent:
                DataReceivedFromServer(receiveNetworkData);
                break;

            case NetworkEventType.DisconnectEvent:                            
                DisconnectedFromServer(receiveNetworkData);
                break;
        }      
    }

    void ConnectedToServer()
    {
        var username = GetUsername();
        SendServerMessage("SetUsername=" + username);

        if (OnConnectedEvent != null)
        {
            OnConnectedEvent(this, EventArgs.Empty);    
        }

        NotificationsServiceController.AddNotification(Color.green, "Успешно се свърза към сървъра!");
    }

    bool IsValidCommand(string command)
    {
        return commands.ContainsKey(command);
    }

    void DataReceivedFromServer(NetworkData networkData)
    {
        var message = networkData.Message;
        var commandName = message.Split('=').First();

        if (IsValidCommand(commandName))
        {
            var commandToExecute = commands[commandName];
            commandToExecute.Invoke(networkData);
        }
        else
        {
            var username = GetUsername();
            var serverConnectionId = networkData.ConnectionId;

            if (OnReceivedDataEvent != null)
            {
                OnReceivedDataEvent(this, new DataSentEventArgs(serverConnectionId, username, message));    
            }    
        }
    }

    void AllowedToConnectToServer(NetworkData networkData)
    {
        isConnected = true;
    }

    void KickedFromServer(NetworkData networkData)
    {
        var message = networkData.Message;
        var commandParams = message.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries)
            .Skip(1)
            .ToArray();
        
        if (commandParams.Length < 1)
        {
            return;
        }

        var kickReason = commandParams.First();
        ShowNotification(Color.red, kickReason);
    }

    void DisconnectedFromServer(NetworkData networkData)
    {
        Disconnect();

        if (OnDisconnectedEvent != null)
        {
            OnDisconnectedEvent(this, EventArgs.Empty);    
        }
    }

    public void ConnectToHost(string ip)
    {
        if (!ip.IsValidIPV4())
        {
            throw new ArgumentException("Invalid ipv4 address");
        }

        //if currently connected, disconnect
        if (isConnected)
        {
            Disconnect();
        }

        NetworkTransport.Init();
        HostTopology topology = new HostTopology(connectionConfig, 2);
        genericHostId = NetworkTransport.AddHost(topology, 0);

        byte error;
        connectionId = NetworkTransport.Connect(genericHostId, ip, Port, 0, out error);

        var networkError = (NetworkConnectionError)error;

        if (networkError != NetworkConnectionError.NoError)
        {
            var errorMessage = NetworkErrorUtils.GetMessage(networkError);
            ShowNotification(Color.red, errorMessage);
            Disconnect();
        }
        else
        {
            isRunning = true;
            StartCoroutine(CheckCommandAllowedToConnectReceivedCoroutine());    
        }
    }

    IEnumerator CheckCommandAllowedToConnectReceivedCoroutine()
    {
        var time = 0f;

        while (time < ReceivePermissionToConnectTimeoutInSeconds)
        {
            if (isConnected)
            {
                ConnectedToServer();
                break;
            }

            time += Time.deltaTime;

            yield return null;
        }

        if (!isConnected)
        {
            Disconnect();
        }
    }

    public void Disconnect()
    {
        byte error = 0;

        isConnected = false;
        isRunning = false;

        try
        {
            NetworkTransport.Disconnect(genericHostId, connectionId, out error);    
        }
        catch (NetworkException ex)
        {
                
        }

        NetworkTransport.RemoveHost(genericHostId);
        NetworkTransport.Shutdown();

        var networkError = (NetworkError)error;

        if (networkError != NetworkError.Ok)
        {
            var errorMessage = NetworkErrorUtils.GetMessage(networkError);
            ShowNotification(Color.red, errorMessage);
        }

        if (OnDisconnectedEvent != null)
        {
            OnDisconnectedEvent(this, EventArgs.Empty);    
        }
    }

    public void SendServerMessage(string data)
    {
        if (!isConnected)
        {
            throw new InvalidOperationException("Not connected to server");
        }

        try
        {
            NetworkTransportUtils.SendMessage(genericHostId, connectionId, communicationChannel, data);    
        }
        catch (NetworkException ex)
        {
            var errorN = ex.ErrorN;
            var error = (NetworkError)errorN;
            var errorMessage = NetworkErrorUtils.GetMessage(error);

            ShowNotification(Color.red, errorMessage);
        }
    }

    #region DEBUG

    #if DEBUG
    string debug_connectIp = "(example 127.0.0.1)";

    void OnGUI()
    {
        GUI.Box(new Rect(0, 0, 300, 300), "ClientNetworkManager debug");

        var connectIpRect = new Rect(5, 30, 130, 25); 
        var connectButtonRect = new Rect(5, 65, 130, 30);

        var connectButton = GUI.Button(connectButtonRect, "Connect");

        debug_connectIp = GUI.TextField(connectIpRect, debug_connectIp);

        if (connectButton)
        {
            ConnectToHost(debug_connectIp);
        }
    }

    #endif
    #endregion
}