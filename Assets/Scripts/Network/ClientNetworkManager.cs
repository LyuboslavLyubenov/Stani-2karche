using System;
using System.Collections;

using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Network
{

    using Assets.Scripts.Commands;
    using Assets.Scripts.Commands.Client;
    using Assets.Scripts.EventArgs;
    using Assets.Scripts.Exceptions;
    using Assets.Scripts.Notifications;
    using Assets.Scripts.Utils;

    using Debug = UnityEngine.Debug;
    using EventArgs = System.EventArgs;

    public class ClientNetworkManager : ExtendedMonoBehaviour
    {
        public const int Port = 7788;

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

        float elapsedTimeSinceNetworkError = 0;
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
                return this.isConnected.Value;
            }
            private set
            {
                this.isConnected.Value = value;

                if (value == true)
                {
                    this.OnConnectedEvent(this, EventArgs.Empty);
                }
                else
                {
                    this.OnDisconnectedEvent(this, EventArgs.Empty);
                }
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
                return this.serverConnectedClientsCount.Value;
            }
        }

        void Awake()
        {
            NetworkTransport.Init();

            this.OnConnectedEvent = delegate
                {
                };

            this.OnReceivedDataEvent = delegate
                {
                };

            this.OnDisconnectedEvent = delegate
                {
                };
        }

        void Start()
        {
            this.ConfigureCommands();
            this.ConfigureClient();

            this.CoroutineUtils.RepeatEverySeconds(SendKeepAliveRequestDelayInSeconds, this.SendKeepAliveRequest);
            this.CoroutineUtils.RepeatEverySeconds(3f, this.BeginReceiveConnectedClientsCount);

            this.StartCoroutine(this.UpdateCoroutine());
        }

        void Update()
        {
            if (!this.IsConnected)
            {
                return;  
            }

            if (this.networkErrorsCount >= MaxNetworkErrorsBeforeDisconnect)
            {
                if (this.elapsedTimeSinceNetworkError >= MaxServerReactionTimeInSeconds)
                {
                    this.Disconnect();
                }

                this.elapsedTimeSinceNetworkError = 0;
                this.networkErrorsCount = 0;
            }

            this.elapsedTimeSinceNetworkError += Time.deltaTime;
        }

        void ConfigureClient()
        {
            this.connectionConfig = new ConnectionConfig();
            this.connectionConfig.MaxConnectionAttempt = MaxConnectionAttempts; 
            this.communicationChannel = this.connectionConfig.AddChannel(QosType.ReliableSequenced);
        }

        void ConfigureCommands()
        {
            if (this.NotificationsServiceController != null)
            {
                this.commandsManager.AddCommand("ShowNotification", new ReceivedNotificationFromServerCommand(this.NotificationsServiceController));    
            }

            var allowedToConnect = new DummyCommand();
            allowedToConnect.OnExecuted += (sender, args) =>
                {
                    this.IsConnected = true;
                };

            this.commandsManager.AddCommand("AllowedToConnect", allowedToConnect);
            this.commandsManager.AddCommand("ConnectedClientsCount", new ReceivedConnectedClientsCountCommand(this.serverConnectedClientsCount));
        }

        void BeginReceiveConnectedClientsCount()
        {
            if (!this.isRunning || !this.IsConnected)
            {
                return;
            }

            var commandData = new NetworkCommandData("ConnectedClientsCount");
            this.SendServerCommand(commandData);
        }

        void SendKeepAliveRequest()
        {
            if (!this.isRunning || !this.IsConnected)
            {
                return;
            }
            
            var commandLine = new NetworkCommandData("KeepAlive");
            this.SendServerCommand(commandLine);
        }

        void ShowNotification(Color color, string message)
        {
            if (this.NotificationsServiceController != null)
            {
                this.NotificationsServiceController.AddNotification(color, message);          
            }
        }

        IEnumerator UpdateCoroutine()
        {
            while (true)
            {
                if (this.isRunning)
                {
                    NetworkTransportUtils.ReceiveMessageAsync(this.ReceivedMessageFromClientAsync, (exception) =>
                        {
                            Debug.LogErrorFormat("NetworkException {0}", (NetworkError)exception.ErrorN);
                            Debug.LogException(exception);
                            this.networkErrorsCount++;
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
                    this.DataReceivedFromServer(networkData);
                    break;

                case NetworkEventType.DisconnectEvent:                            
                    this.DisconnectedFromServer(networkData);
                    break;
            } 
        }

        void ConnectedToServer()
        {
            var username = this.GetUsername();
            var commandLine = new NetworkCommandData("SetUsername");

            commandLine.AddOption("Username", username);
            this.SendServerCommand(commandLine);

            this.IsConnected = true;
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
                    this.commandsManager.Execute(commandLine);    
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            }
            else
            {
                var username = this.GetUsername();
                var serverConnectionId = networkData.ConnectionId;

                if (this.OnReceivedDataEvent != null)
                {
                    this.OnReceivedDataEvent(this, new DataSentEventArgs(serverConnectionId, username, message));    
                }    
            }
        }

        void DisconnectedFromServer(NetworkData networkData)
        {
            this.Disconnect();
        }

        public void ConnectToHost(string ip)
        {
            if (!ip.IsValidIPV4())
            {
                throw new ArgumentException("Invalid ipv4 address");
            }

            Debug.Log("Connecting to " + ip);

            if (this.IsConnected)
            {
                this.Disconnect();
            }

            HostTopology topology = new HostTopology(this.connectionConfig, 2);
            this.genericHostId = NetworkTransport.AddHost(topology, 0);

            byte error;
            this.connectionId = NetworkTransport.Connect(this.genericHostId, ip, Port, 0, out error);

            var networkError = (NetworkConnectionError)error;

            if (networkError != NetworkConnectionError.NoError)
            {
                var errorMessage = NetworkErrorUtils.GetMessage(networkError);
                Debug.LogError(errorMessage);
                this.Disconnect();
            }
            else
            {
                this.isRunning = true;
                this.StartCoroutine(this.CheckCommandAllowedToConnectReceivedCoroutine());
            }
        }

        IEnumerator CheckCommandAllowedToConnectReceivedCoroutine()
        {
            var time = 0f;

            while (time < ReceivePermissionToConnectTimeoutInSeconds)
            {
                if (this.IsConnected)
                {
                    this.ConnectedToServer();
                    break;
                }

                time += Time.deltaTime;

                yield return null;
            }

            if (!this.IsConnected)
            {
                this.Disconnect();
            }
        }

        public void Disconnect()
        {
            byte error = 0;

            try
            {
                NetworkTransport.Disconnect(this.genericHostId, this.connectionId, out error);    
            }
            catch (NetworkException ex)
            {
                Debug.Log(ex.Message);
            }

            NetworkTransport.RemoveHost(this.genericHostId);

            var networkError = (NetworkError)error;

            if (networkError != NetworkError.Ok)
            {
                var errorMessage = NetworkErrorUtils.GetMessage(networkError);
                this.ShowNotification(Color.red, errorMessage);
            }

            this.IsConnected = false;
            this.isRunning = false;
        }

        public void SendServerCommand(NetworkCommandData commandData)
        {
            var commandString = commandData.ToString();
            this.SendServerMessage(commandString);
        }

        public void SendServerMessage(string data)
        {
            if (!this.isConnected)
            {
                throw new InvalidOperationException("Not connected to server");
            }

            NetworkTransportUtils.SendMessageAsync(this.genericHostId, this.connectionId, this.communicationChannel, data, (exception) =>
                {
                    var errorN = exception.ErrorN;
                    var error = (NetworkError)errorN;
                    var errorMessage = NetworkErrorUtils.GetMessage(error);
                    Debug.LogException(new Exception(errorMessage));

                    this.networkErrorsCount++;
                });
        }


        #region DEBUG

        string debug_connectIp = "(example 127.0.0.1)";

        void OnGUI()
        {
            if (!this.ShowDebugMenu)
            {
                return;
            }

            GUI.Box(new Rect(200, 0, 300, 300), "ClientNetworkManager debug");

            var connectIpRect = new Rect(205, 30, 130, 25); 
            var connectButtonRect = new Rect(205, 65, 130, 30);

            var connectButton = GUI.Button(connectButtonRect, "Connect");

            this.debug_connectIp = GUI.TextField(connectIpRect, this.debug_connectIp);

            if (connectButton)
            {
                this.ConnectToHost(this.debug_connectIp);
            }
        }

        #endregion
    }

}