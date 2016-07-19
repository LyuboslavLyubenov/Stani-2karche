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

    public NotificationsServiceController NotificationsServiceController;
    //how many times to try to connect to server before disconnecting and start searching for another (only if LANbroadcastService is present)
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

    bool isRunning = false;

    Dictionary<string, Action<NetworkData>> commands = new Dictionary<string, Action<NetworkData>>();

    public bool IsRunning
    {
        get
        {
            return isRunning;
        }
    }

    void Start()
    {
        ConfigureCommands();
        ConfigureClient();
        StartCoroutine(UpdateCoroutine());
    }

    /// <summary>
    /// Configures the client connection settings.
    /// </summary>
    void ConfigureClient()
    {
        connectionConfig = new ConnectionConfig();
        connectionConfig.MaxConnectionAttempt = RetriesBeforeSearchingForAnotherServer; 
        communicationChannel = connectionConfig.AddChannel(QosType.ReliableSequenced); //make sure messages are delivered and send in correct order
    }

    void ConfigureCommands()
    {
        commands["KickReason"] = KickedFromServer;
    }

    void ShowNotification(Color color, string message)
    {
        if (NotificationsServiceController != null)
        {
            NotificationsServiceController.AddNotification(color, message);          
        }
    }

    IEnumerator OnServerFoundCoroutine(string address)
    {
        bool successfullyConnected = false;

        for (int i = 0; i < RetriesBeforeSearchingForAnotherServer; i++)
        {
            //try to connect
            ConnectToHost(address);

            yield return new WaitForSeconds(0.5f);

            //if connected dont try again
            if (isRunning)
            {
                successfullyConnected = true;
                break;
            }
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
        NetworkData receiveNetworkData = null;

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
            case NetworkEventType.ConnectEvent:
                ConnectedToServer(receiveNetworkData);
                break;

            case NetworkEventType.DataEvent:
                DataReceivedFromServer(receiveNetworkData);
                break;

            case NetworkEventType.DisconnectEvent:                            
                DisconnectedFromServer(receiveNetworkData);
                break;
        }      
    }

    void ConnectedToServer(NetworkData networkData)
    {
        var username = GetUsername();
        SendServerMessage("SetUsername=" + username);

        CoroutineUtils.WaitForFrames(3, () =>
            {
                
            });

        if (OnConnectedEvent != null)
        {
            OnConnectedEvent(this, EventArgs.Empty);    
        }
    }

    bool IsValidCommand(string command)
    {
        return commands.ContainsKey(command);
    }

    void DataReceivedFromServer(NetworkData networkData)
    {
        var message = networkData.Message;
        var commandName = message.Split('=')[0];

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
        NetworkTransport.Shutdown();
        isRunning = false;

        if (OnDisconnectedEvent != null)
        {
            OnDisconnectedEvent(this, EventArgs.Empty);    
        }
    }

    public void ConnectToHost(string ip)
    {
        if (string.IsNullOrEmpty(ip) && ip.Length < 4)
        {
            throw new ArgumentOutOfRangeException("ip", "Invalid ip address length");
        }
            
        var ipDigits = ip.Split('.');
       
        if (ipDigits.Length != 4)
        {
            throw new ArgumentException("Invalid ip address");
        }

        //if currently connected, disconnect
        if (isRunning)
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
        }
    }

    public void Disconnect()
    {
        byte error;

        isRunning = false;

        NetworkTransport.Disconnect(genericHostId, connectionId, out error);
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

    string debug_connectIp = "";

    void OnGUI()
    {
        GUI.Box(new Rect(0, 0, 300, 300), "ClientNetworkManager debug");

        var connectButtonRect = new Rect(5, 80, 100, 30);
        var connectIpRect = new Rect(5, 30, 100, 30); 

        var connectButton = GUI.Button(connectButtonRect, "Connect");

        debug_connectIp = GUI.TextField(connectIpRect, debug_connectIp);

        if (connectButton)
        {
            ConnectToHost(debug_connectIp);
        }
    }
}