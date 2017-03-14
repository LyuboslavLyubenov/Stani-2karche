namespace Assets.Scripts.Network.NetworkManagers
{
    using System;
    using System.Collections;
    using System.Timers;

    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Interfaces.Network.NetworkManager;

    using Extensions;

    using Commands;
    using Commands.Client;
    using Commands.Server;
    using DTOs;
    using EventArgs;
    using Exceptions;
    using Notifications;
    using Utils;
    using Utils.Unity;

    using UnityEngine;
    using UnityEngine.Networking;

    using Debug = UnityEngine.Debug;
    using EventArgs = System.EventArgs;

    public class ClientNetworkManager : IDisposable, IClientNetworkManager
    {
        public const int Port = 7788;

        private const float ReceiveNetworkMessagesDelayInSeconds = 0.5f;
        private const float ReceivePermissionToConnectTimeoutInSeconds = 6f;
        private const float SendKeepAliveRequestDelayInSeconds = 3f;
        private const float ReceiveConnectedClientsCountDelayInSeconds = 3f;

        private const int MaxConnectionAttempts = 3;
        private const int MaxServerReactionTimeInSeconds = 6;
        private const int MaxNetworkErrorsBeforeDisconnect = 5;

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

        private static ClientNetworkManager instance;

        public static ClientNetworkManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ClientNetworkManager();
                }

                return instance;
            }
        }

        public event EventHandler OnConnectedEvent;

        public event EventHandler<DataSentEventArgs> OnReceivedDataEvent;

        public event EventHandler OnDisconnectedEvent;

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

        private Timer keepAliveTimer;
        private Timer connectedClientsCountTimer;
        private Timer receiveNetworkMessagesTimer;
        private Timer validateConnectionTimer;

        public ClientNetworkManager()
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

            this.ConfigureCommands();
            this.ConfigureClient();

            this.keepAliveTimer =
                TimerUtils.ExecuteEvery(SendKeepAliveRequestDelayInSeconds, this.SendKeepAliveRequest);

            this.connectedClientsCountTimer =
                TimerUtils.ExecuteEvery(ReceiveConnectedClientsCountDelayInSeconds, this.BeginReceiveConnectedClientsCount);

            this.receiveNetworkMessagesTimer =
                TimerUtils.ExecuteEvery(ReceiveNetworkMessagesDelayInSeconds, this.ReceiveMessages);

            this.validateConnectionTimer =
                TimerUtils.ExecuteEvery(1f, this.ValidateConnection);

            ((IExtendedTimer)this.keepAliveTimer).RunOnUnityThread = true;
            ((IExtendedTimer)this.connectedClientsCountTimer).RunOnUnityThread = true;
            ((IExtendedTimer)this.receiveNetworkMessagesTimer).RunOnUnityThread = true;
            ((IExtendedTimer)this.validateConnectionTimer).RunOnUnityThread = true;

            this.keepAliveTimer.Start();
            this.connectedClientsCountTimer.Start();
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
            this.commandsManager.AddCommand("ShowNotification", new NotificationFromServerCommand(NotificationsesController.Instance));

            var allowedToConnect = new DummyCommand();
            allowedToConnect.OnExecuted += (sender, args) =>
                {
                    this.IsConnected = true;
                };

            this.commandsManager.AddCommand("AllowedToConnect", allowedToConnect);
            this.commandsManager.AddCommand("ConnectedClientsCount", new ClientReceiveConnectedClientsCountCommand(this.serverConnectedClientsCount));
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
                NetworkTransportUtils.ReceiveMessageAsync(this.ReceivedMessageFromClientAsync, (exception) =>
                    {
                        Debug.LogErrorFormat("NetworkException {0}", (NetworkError)exception.ErrorN);
                        Debug.LogException(exception);
                        this.networkErrorsCount++;
                    });
            }
        }

        private string GetUsername()
        {
            var username = string.Empty;

            if (PlayerPrefsEncryptionUtils.HasKey("Username"))
            {
                username = PlayerPrefsEncryptionUtils.GetString("Username");
            }

            return username;
        }

        private void ReceivedMessageFromClientAsync(NetworkData networkData)
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

        private void ConnectedToServer()
        {
            var username = this.GetUsername();
            var commandLine = NetworkCommandData.From<SetUsernameCommand>();

            commandLine.AddOption("Username", username);

            this.SendServerCommand(commandLine);

            this.IsConnected = true;
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

        public void Dispose()
        {
            try
            {
                this.Disconnect();

                this.keepAliveTimer.Stop();
                this.connectedClientsCountTimer.Stop();
                this.receiveNetworkMessagesTimer.Stop();
                this.validateConnectionTimer.Stop();
            }
            finally
            {
                this.keepAliveTimer.Dispose();
                this.connectedClientsCountTimer.Dispose();
                this.receiveNetworkMessagesTimer.Dispose();
                this.validateConnectionTimer.Dispose();

                this.keepAliveTimer = null;
                this.connectedClientsCountTimer = null;
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