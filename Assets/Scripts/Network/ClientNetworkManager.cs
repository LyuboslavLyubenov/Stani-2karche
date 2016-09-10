using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// Client network manager.
/// </summary>
public class ClientNetworkManager : ExtendedMonoBehaviour
{
    const int Port = 7788;
    const float ReceivePermissionToConnectTimeoutInSeconds = 3f;
    const float SendKeepAliveRequestDelayInSeconds = 1f;
    const int MaxConnectionAttempts = 3;

    public NotificationsServiceController NotificationsServiceController;

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

    ValueWrapper<bool> isConnected = new ValueWrapper<bool>(false);
    bool isRunning = false;

    CommandsManager commandsManager = new CommandsManager();

    ValueWrapper<int> serverConnectedClientsCount = new ValueWrapper<int>();

    public bool IsConnected
    {
        get
        {
            return isConnected.Value;
        }
    }

    public CommandsManager CommandsManager
    {
        get
        {
            return this.commandsManager; 
        }
    }

    public int ServerConnectedClientsCount
    {
        get
        {
            return serverConnectedClientsCount.Value;
        }
    }

    void Start()
    {
        ConfigureCommands();
        ConfigureClient();

        CoroutineUtils.RepeatEverySeconds(SendKeepAliveRequestDelayInSeconds, SendKeepAliveRequest);
        CoroutineUtils.RepeatEverySeconds(1f, () =>
            {
                if (!IsConnected)
                {
                    return;    
                }

                var commandData = new NetworkCommandData("ConnectedClientsCount");
                SendServerCommand(commandData);
            });

        StartCoroutine(UpdateCoroutine());
    }

    void ConfigureClient()
    {
        connectionConfig = new ConnectionConfig();
        connectionConfig.MaxConnectionAttempt = MaxConnectionAttempts; 
        communicationChannel = connectionConfig.AddChannel(QosType.ReliableSequenced); //make sure messages are delivered and send in correct order
    }

    void ConfigureCommands()
    {
        commandsManager.AddCommand("ShowNotification", new ShowNotificationFromServerCommand(NotificationsServiceController));
        commandsManager.AddCommand("AllowedToConnect", new AllowToConnectToServerCommand(isConnected));
        commandsManager.AddCommand("ConnectedClientsCount", new ClientReceiveConnectedClientsCountCommand(serverConnectedClientsCount));
    }

    void SendKeepAliveRequest()
    {
        if (isRunning && IsConnected)
        {
            var commandLine = new NetworkCommandData("KeepAlive");
            SendServerCommand(commandLine);
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
        var commandLine = new NetworkCommandData("SetUsername");

        commandLine.AddOption("Username", username);
        SendServerCommand(commandLine);

        if (OnConnectedEvent != null)
        {
            OnConnectedEvent(this, EventArgs.Empty);    
        }

        NotificationsServiceController.AddNotification(Color.green, "Успешно се свърза към сървъра!");
    }

    void DataReceivedFromServer(NetworkData networkData)
    {
        var message = networkData.Message;
        NetworkCommandData commandLine = null;

        try
        {
            commandLine = NetworkCommandData.Parse(message);
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }

        if (commandLine != null)
        {
            commandsManager.Execute(commandLine);
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
        if (IsConnected)
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
            if (IsConnected)
            {
                ConnectedToServer();
                break;
            }

            time += Time.deltaTime;

            yield return null;
        }

        if (!IsConnected)
        {
            Disconnect();
        }
    }

    public void Disconnect()
    {
        byte error = 0;

        isConnected.Value = false;
        isRunning = false;

        try
        {
            NetworkTransport.Disconnect(genericHostId, connectionId, out error);    
        }
        catch (NetworkException ex)
        {
            Debug.Log(ex.Message);
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

    public void SendServerCommand(NetworkCommandData commandLine)
    {
        SendServerMessage(commandLine.ToString());
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

            Debug.LogException(ex);
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