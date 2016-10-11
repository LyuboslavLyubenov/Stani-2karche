using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System;

public class ClientNetworkManager : ExtendedMonoBehaviour, IClientNetworkManager
{
    const int Port = 7788;

    const float ReceivePermissionToConnectTimeoutInSeconds = 6f;
    const float SendKeepAliveRequestDelayInSeconds = 3f;

    const int MaxConnectionAttempts = 3;

    const int MaxServerReactionTimeInSeconds = 6;
    const int MaxNetworkErrorsBeforeDisconnect = 5;

    public NotificationsServiceController NotificationsServiceController;
    public bool ShowDebugMenu = false;

    int connectionId = 0;
    int genericHostId = 0;

    ConnectionConfig connectionConfig = null;
    byte communicationChannel = 0;

    ValueWrapper<bool> isConnected = new ValueWrapper<bool>(false);
    bool isRunning = false;

    CommandsManager commandsManager = new CommandsManager();

    ValueWrapper<int> serverConnectedClientsCount = new ValueWrapper<int>();

    float elapsedTimeSinceFirstNetworkError = 0;
    int networkErrorsCount = 0;


    public EventHandler OnConnectedEvent
    {
        get;
        set;
    }

    public EventHandler<DataSentEventArgs> OnReceivedDataEvent
    {
        get;
        set;
    }

    public EventHandler OnDisconnectedEvent
    {
        get;
        set;
    }

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
        #if UNITY_EDITOR
        PlayerPrefs.SetString("ServerIP", "127.0.0.1");
        #endif

        OnConnectedEvent = delegate
        {
        };

        OnReceivedDataEvent = delegate
        {
        };

        OnDisconnectedEvent = delegate
        {
        };

        ConfigureCommands();
        ConfigureClient();

        CoroutineUtils.RepeatEverySeconds(SendKeepAliveRequestDelayInSeconds, SendKeepAliveRequest);
        CoroutineUtils.RepeatEverySeconds(3f, BeginReceiveConnectedClientsCount);

        StartCoroutine(UpdateCoroutine());
    }

    void Update()
    {
        if (networkErrorsCount >= MaxNetworkErrorsBeforeDisconnect)
        {
            if (elapsedTimeSinceFirstNetworkError >= MaxServerReactionTimeInSeconds)
            {
                Disconnect();
            }

            elapsedTimeSinceFirstNetworkError = 0;
            networkErrorsCount = 0;
        }

        elapsedTimeSinceFirstNetworkError += Time.deltaTime;
    }

    void ConfigureClient()
    {
        connectionConfig = new ConnectionConfig();
        connectionConfig.MaxConnectionAttempt = MaxConnectionAttempts; 
        communicationChannel = connectionConfig.AddChannel(QosType.ReliableFragmented);
    }

    void ConfigureCommands()
    {
        commandsManager.AddCommand("ShowNotification", new ReceivedNotificationFromServerCommand(NotificationsServiceController));
        commandsManager.AddCommand("AllowedToConnect", new ReceivedAllowToConnectToServerCommand(isConnected));
        commandsManager.AddCommand("ConnectedClientsCount", new ReceivedConnectedClientsCountCommand(serverConnectedClientsCount));
    }

    void OnNetworkError()
    {
        if (networkErrorsCount == 0)
        {
            elapsedTimeSinceFirstNetworkError = 0;    
        }

        networkErrorsCount++;
    }

    void BeginReceiveConnectedClientsCount()
    {
        if (!isRunning || !IsConnected)
        {
            return;
        }

        var commandData = new NetworkCommandData("ConnectedClientsCount");
        SendServerCommand(commandData);
    }

    void SendKeepAliveRequest()
    {
        if (!isRunning || !IsConnected)
        {
            return;
        }
            
        var commandLine = new NetworkCommandData("KeepAlive");
        SendServerCommand(commandLine);
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
                NetworkTransportUtils.ReceiveMessageAsync(ReceivedMessageFromClientAsync, (exception) =>
                    {
                        Debug.LogErrorFormat("NetworkException {0}", (NetworkError)exception.ErrorN);
                        Debug.LogException(exception);
                    });
            }

            yield return new WaitForSeconds(0.5f);
        }
    }

    string GetUsername()
    {
        var username = "Anonymous";

        if (PlayerPrefsEncryptionUtils.HasKey("Username"))
        {
            username = PlayerPrefsEncryptionUtils.GetString("Username");
        }

        return username;
    }

    void ReceivedMessageFromClientAsync(NetworkData networkData)
    {
        switch (networkData.NetworkEventType)
        {
            case NetworkEventType.DataEvent:
                DataReceivedFromServer(networkData);
                break;

            case NetworkEventType.DisconnectEvent:                            
                DisconnectedFromServer(networkData);
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
            try
            {
                commandsManager.Execute(commandLine);    
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
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
            Debug.LogError(errorMessage);
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

    public void SendServerCommand(NetworkCommandData commandData)
    {
        var commandString = commandData.ToString();
        SendServerMessage(commandString);
    }

    public void SendServerMessage(string data)
    {
        if (!isConnected)
        {
            throw new InvalidOperationException("Not connected to server");
        }

        NetworkTransportUtils.SendMessageAsync(genericHostId, connectionId, communicationChannel, data, (exception) =>
            {
                var errorN = exception.ErrorN;
                var error = (NetworkError)errorN;
                var errorMessage = NetworkErrorUtils.GetMessage(error);
                Debug.LogException(new Exception(errorMessage));

                OnNetworkError();
            });
    }

    #region DEBUG

    string debug_connectIp = "(example 127.0.0.1)";

    void OnGUI()
    {
        if (!ShowDebugMenu)
        {
            return;
        }

        GUI.Box(new Rect(200, 0, 300, 300), "ClientNetworkManager debug");

        var connectIpRect = new Rect(205, 30, 130, 25); 
        var connectButtonRect = new Rect(205, 65, 130, 30);

        var connectButton = GUI.Button(connectButtonRect, "Connect");

        debug_connectIp = GUI.TextField(connectIpRect, debug_connectIp);

        if (connectButton)
        {
            ConnectToHost(debug_connectIp);
        }
    }

    #endregion
}