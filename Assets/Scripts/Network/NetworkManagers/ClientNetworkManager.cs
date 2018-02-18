namespace Network.NetworkManagers
{
    using System;
    using System.Collections;
    using System.Timers;

    using Assets.Scripts.Commands.Client;
    using Assets.Scripts.Extensions;

    using Commands;
    using Commands.Client;
    using Commands.Server;

    using DTOs;

    using EventArgs;

    using Interfaces.Network.NetworkManager;

    using Notifications;
    
    using UnityEngine;
    using UnityEngine.Networking;

    using Utils;
    using Utils.Unity;

    using Debug = UnityEngine.Debug;
    using EventArgs = System.EventArgs;

    public class ClientNetworkManager : IDisposable, IClientNetworkManager
    {
        public const int Port = 7788;

        private const float ReceiveNetworkMessagesDelayInSeconds = 1f;
        private const float ReceivePermissionToConnectTimeoutInSeconds = 6f;
        private const float SendKeepAliveRequestDelayInSeconds = 3f;
        private const float ValidateConnectionDelayInSeconds = 1f;

        private const int MaxConnectionAttempts = 10;
        private const int MaxServerReactionTimeInSeconds = 10;
        private const int MaxNetworkErrorsBeforeDisconnect = 5;
        
        public event EventHandler OnConnectedEvent = delegate { };
        public event EventHandler<DataSentEventArgs> OnReceivedDataEvent = delegate { };
        public event EventHandler OnDisconnectedEvent = delegate { };

        public bool ShowDebugMenu = false;
        
        private int connectionId = 0;
        private int genericHostId = 0;
        private ConnectionConfig connectionConfig = null;
        private byte communicationChannel = 0;

        private ValueWrapper<bool> isConnected = new ValueWrapper<bool>(false);

        private bool isRunning = false;

        private CommandsManager commandsManager = new CommandsManager();

        private ValueWrapper<int> serverConnectedClientsCount = new ValueWrapper<int>();

        private int elapsedTimeSinceNetworkError = 0;
        private int networkErrorsCount = 0;

        private string encryptionKey = "";
        
        private Timer_ExecuteMethodEverySeconds keepAliveTimer;
        private Timer_ExecuteMethodEverySeconds receiveNetworkMessagesTimer;
        private Timer_ExecuteMethodEverySeconds validateConnectionTimer;

        private static ClientNetworkManager instance;

        public static ClientNetworkManager Instance
        {
            get
            {
                return instance ?? (instance = new ClientNetworkManager());
            }
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

        private ClientNetworkManager()
        {
            NetworkTransport.Init();
            
            this.ConfigureCommands();
            this.ConfigureClient();

            this.keepAliveTimer =
                TimerUtils.ExecuteEvery(SendKeepAliveRequestDelayInSeconds, this.SendKeepAliveRequest);

            this.receiveNetworkMessagesTimer =
                TimerUtils.ExecuteEvery(ReceiveNetworkMessagesDelayInSeconds, this.ReceiveMessages);

            this.validateConnectionTimer =
                TimerUtils.ExecuteEvery(ValidateConnectionDelayInSeconds, this.ValidateConnection);

            this.keepAliveTimer.RunOnUnityThread = true;
            this.receiveNetworkMessagesTimer.RunOnUnityThread = true;
            this.validateConnectionTimer.RunOnUnityThread = true;

            this.keepAliveTimer.Start();
            this.receiveNetworkMessagesTimer.Start();
            this.validateConnectionTimer.Start();
        }

        private void ValidateConnection()
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

            this.elapsedTimeSinceNetworkError++;
        }

        private void ConfigureClient()
        {
            this.connectionConfig = new ConnectionConfig();
            this.connectionConfig.MaxConnectionAttempt = MaxConnectionAttempts;
            this.communicationChannel = this.connectionConfig.AddChannel(QosType.ReliableSequenced);
        }

        private void ConfigureCommands()
        {
            this.commandsManager.AddCommand("ShowNotification", new NotificationFromServerCommand(NotificationsController.Instance));

            var allowedToConnect = new DummyCommand();
            allowedToConnect.OnExecuted += (sender, args) =>
                {
                    this.IsConnected = true;
                };

            this.commandsManager.AddCommand("AllowedToConnect", allowedToConnect);
            this.commandsManager.AddCommand(new SendDeviceIdToServerCommand(this));
        }

        private void BeginReceiveConnectedClientsCount()
        {
            if (!this.isRunning || !this.IsConnected)
            {
                return;
            }

            var commandData = NetworkCommandData.From<ServerSendConnectedClientsCountCommand>();
            this.SendServerCommand(commandData);
        }

        private void SendKeepAliveRequest()
        {
            if (!this.isRunning || !this.IsConnected)
            {
                return;
            }

            var commandLine = NetworkCommandData.From<KeepAliveCommand>();
            this.SendServerCommand(commandLine);
        }
        
        private void ReceiveMessages()
        {
            if (this.isRunning)
            {
                NetworkTransportUtils.ReceiveMessageAsync(this.ReceivedMessageFromServerAsync, 
                    (exception) =>
                    {
                        Debug.LogErrorFormat("NetworkException {0}", (NetworkError)exception.ErrorN);
                        Debug.LogException(exception);
                        this.networkErrorsCount++;
                    });
            }
        }

        private string GetUsername()
        {
            #if UNITY_EDITOR || UNITY_EDITOR_64 || UNITY_EDITOR_LINUX || DEVELOPMENT_BUILD
            var username = Guid.NewGuid().ToString();
            #else
            var username = SystemInfo.deviceUniqueIdentifier;
            #endif

            if (PlayerPrefsEncryptionUtils.HasKey("Username"))
            {
                username = PlayerPrefsEncryptionUtils.GetString("Username");
            }

            return username;
        }

        private void ReceivedMessageFromServerAsync(NetworkData networkData)
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

        private IEnumerator ConnectedToServerCoroutine()
        {
            var username = this.GetUsername();
            var setUsernameCommand = NetworkCommandData.From<SetUsernameCommand>();
            setUsernameCommand.AddOption("Username", username);
            this.SendServerCommand(setUsernameCommand);

            yield return null;

            this.IsConnected = true;
        }

        private void ConnectedToServer()
        {
            ThreadUtils.Instance.RunOnMainThread(this.ConnectedToServerCoroutine());
        }

        private void DataReceivedFromServer(NetworkData networkData)
        {
            var message = networkData.Message;
            NetworkCommandData commandLine = null;

            try
            {
                commandLine = NetworkCommandData.Parse(message);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
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
                var serverConnectionId = networkData.ConnectionId;

                if (this.OnReceivedDataEvent != null)
                {
                    this.OnReceivedDataEvent(this, new DataSentEventArgs(serverConnectionId, message));
                }
            }
        }

        private void DisconnectedFromServer(NetworkData networkData)
        {
            this.Disconnect();
        }

        private IEnumerator CheckCommandAllowedToConnectReceivedCoroutine()
        {
            var time = 0f;

            while (time < ReceivePermissionToConnectTimeoutInSeconds)
            {
                if (this.IsConnected)
                {
                    this.ConnectedToServer();
                    yield break;
                }

                time += Time.deltaTime;

                yield return null;
            }

            if (!this.IsConnected)
            {
                this.Disconnect();
            }
        }
        
        public NetworkConnectionError ConnectToHost(string ip)
        {
            if (!ip.IsValidIPV4())
            {
                throw new ArgumentException("Invalid ipv4 address");
            }

            if (this.IsConnected)
            {
                this.Disconnect();
            }

            HostTopology topology = new HostTopology(this.connectionConfig, 2);
            this.genericHostId = NetworkTransport.AddHost(topology, 0);

            byte error;
            this.connectionId = NetworkTransport.Connect(this.genericHostId, ip, Port, 0, out error);

            var networkError = (NetworkConnectionError)error;

            if (networkError == NetworkConnectionError.NoError)
            {
                this.isRunning = true;
                ThreadUtils.Instance.RunOnMainThread(this.CheckCommandAllowedToConnectReceivedCoroutine());
            }
            else
            {
                this.Disconnect();
            }

            return networkError;
        }

        public NetworkError Disconnect()
        {
            byte error = 0;

            NetworkTransport.Disconnect(this.genericHostId, this.connectionId, out error);
            NetworkTransport.RemoveHost(this.genericHostId);

            this.IsConnected = false;
            this.isRunning = false;

            var networkError = (NetworkError)error;
            return networkError;
        }

        public void SendServerCommand(NetworkCommandData commandData)
        {
            var commandString = commandData.ToString();
            this.SendServerMessage(commandString);
        }

        public void SendServerMessage(string data)
        {
            NetworkTransportUtils.SendMessageAsync(
                this.genericHostId, 
                this.connectionId, 
                this.communicationChannel, 
                data, 
                (exception) =>
                {
                    var errorN = exception.ErrorN;
                    var error = (NetworkError)errorN;
                    var errorMessage = NetworkErrorUtils.GetMessage(error);
                    Debug.LogException(new Exception(errorMessage));

                    this.networkErrorsCount++;
                });
        }

        public void Dispose()
        {
            try
            {
                this.Disconnect();

                this.keepAliveTimer.Stop();
                this.receiveNetworkMessagesTimer.Stop();
                this.validateConnectionTimer.Stop();
            }
            finally
            {
                this.keepAliveTimer.Dispose();
                this.receiveNetworkMessagesTimer.Dispose();
                this.validateConnectionTimer.Dispose();

                this.keepAliveTimer = null;
                this.receiveNetworkMessagesTimer = null;
                this.validateConnectionTimer = null;

                instance = null;
            }
        }

#region DEBUG

        private string debug_connectIp = "(example 127.0.0.1)";
        
        private void OnGUI()
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