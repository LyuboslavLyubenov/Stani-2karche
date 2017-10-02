namespace Network.NetworkManagers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Timers;

    using Assets.Scripts.Commands.Client;
    using Assets.Scripts.Utils;

    using Commands;
    using Commands.Server;

    using DTOs;

    using EventArgs;

    using Exceptions;

    using Interfaces.Network.NetworkManager;

    using Localization;

    using Network.Broadcast;

    using UnityEngine;
    using UnityEngine.Networking;

    using Utils;
    using Utils.Unity;

    using Debug = UnityEngine.Debug;

    public class ServerNetworkManager : IServerNetworkManager, IDisposable
    {
        private const int Port = 7788;

        private const float CheckForDeadClientsDelayInSeconds = 5f;

        private const float ReceiveDeviceIdMaxTimeInSeconds = 8f;

        public bool AutoStart = true;
        //how many clients can be connected to the server
        public int MaxConnections
        {
            get;
            set;
        }

        public event EventHandler<ClientConnectionIdEventArgs> OnClientConnected = delegate
            {
            };

        public event EventHandler<DataSentEventArgs> OnReceivedData = delegate
            {
            };

        public event EventHandler<ClientConnectionIdEventArgs> OnClientDisconnected = delegate
            {
            };

        public event EventHandler<ConnectedClientDataEventArgs> OnClientSetUsername = delegate
            {
            };

        private int genericHostId = 0;
        private ConnectionConfig connectionConfig = null;
        private HostTopology topology = null;
        private byte communicationChannel = 0;

        private bool isRunning = false;
        //Id of all connected clients
        private readonly List<int> connectedClientsIds = new List<int>();
        private readonly List<string> bannedDeviceIds = new List<string>();
        private readonly HashSet<int> aliveClientsId = new HashSet<int>();
        private readonly Dictionary<int, string> connectedClientsNames = new Dictionary<int, string>();
        private readonly Dictionary<int, string> connectedClientsDeviceIds = new Dictionary<int, string>();
        private readonly CommandsManager commandsManager = new CommandsManager();
        private readonly LANServerOnlineBroadcastService LANServerOnlineBroadcastService = new LANServerOnlineBroadcastService();

        private Timer updateAliveClientsTimer;

        private static ServerNetworkManager instance;

        public static ServerNetworkManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ServerNetworkManager();
                }

                return instance;
            }
        }

        public bool IsRunning
        {
            get
            {
                return this.isRunning;
            }
        }

        public int ConnectedClientsCount
        {
            get
            {
                return this.connectedClientsIds.Count;
            }
        }

        public int[] ConnectedClientsConnectionId
        {
            get
            {
                return this.connectedClientsIds.ToArray();
            }
        }

        public string[] BannedClientsConnectionIds
        {
            get
            {
                return this.bannedDeviceIds.ToArray();
            }
        }

        public string[] GetAllClientsNames()
        {
            return this.connectedClientsNames.Select(c => c.Value).ToArray();
        }

        public ICommandsManager CommandsManager
        {
            get
            {
                return this.commandsManager;
            }
        }

        private ServerNetworkManager()
        {
            this.MaxConnections = 30;

            this.ConfigureCommands();
            this.ConfigureServer();

            if (this.AutoStart)
            {
                this.StartServer();
            }
            
            this.updateAliveClientsTimer = TimerUtils.ExecuteEvery(
                CheckForDeadClientsDelayInSeconds,
                this.UpdateAliveClients);

            ((IExtendedTimer)this.updateAliveClientsTimer).RunOnUnityThread = true;
            this.updateAliveClientsTimer.Start();

            ThreadUtils.Instance.RunOnMainThread(this.ReceiveMessagesCoroutine());
        }

        private void ConfigureServer()
        {
            NetworkTransport.Init();

            this.connectionConfig = new ConnectionConfig();
            this.communicationChannel = this.connectionConfig.AddChannel(QosType.ReliableSequenced);
            this.topology = new HostTopology(this.connectionConfig, this.MaxConnections);
        }

        private void ConfigureCommands()
        {
            this.commandsManager.AddCommand(new SetClientIdCommand(this.OnReceivedClientId));
            this.commandsManager.AddCommand(new SetUsernameCommand(this));
            this.commandsManager.AddCommand(new KeepAliveCommand(this.aliveClientsId));
            this.commandsManager.AddCommand(new ServerSendConnectedClientsIdsNamesCommand(this, this.connectedClientsNames));
        }

        private void OnReceivedClientId(int connectionId, string deviceId)
        {
            if (this.aliveClientsId.Contains(connectionId) || 
                this.connectedClientsDeviceIds.ContainsKey(connectionId) ||
                this.bannedDeviceIds.Contains(deviceId) || 
                this.connectedClientsDeviceIds.ContainsValue(deviceId))
            {
                return;
            }
            
            var commandData = new NetworkCommandData("AllowedToConnect");
            this.SendClientCommand(connectionId, commandData);
            
            this.connectedClientsDeviceIds.Add(connectionId, deviceId);
            this.aliveClientsId.Add(connectionId);
            this.connectedClientsIds.Add(connectionId);

            if (this.OnClientConnected != null)
            {
                this.OnClientConnected(this, new ClientConnectionIdEventArgs(connectionId));
            }
        }

        private IEnumerator ReceiveMessagesCoroutine()
        {
            while (true)
            {
                if (this.isRunning)
                {
                    this.ReceiveMessage();
                }

                yield return null;
            }
        }

        private void ReceiveMessage()
        {
            NetworkTransportUtils.ReceiveMessageAsync(this.ReceivedDataFromClientAsync, (exception) =>
                {
                    var error = (NetworkError)exception.ErrorN;
                    var errorMessage = NetworkErrorUtils.GetMessage(error);

                    Debug.LogError(errorMessage);
                });
        }

        private void ReceivedDataFromClientAsync(NetworkData networkData)
        {
            switch (networkData.NetworkEventType)
            {
                case NetworkEventType.ConnectEvent:
                    this.OnConnectedClient(networkData);
                    break;

                case NetworkEventType.DataEvent:
                    this.OnClientSendData(networkData);
                    break;

                case NetworkEventType.DisconnectEvent:
                    this.OnClientDisconnect(networkData);
                    break;
            }
        }

        private IList<int> GetDeadClientsIds()
        {
            var result = new List<int>();

            for (int i = 0; i < this.connectedClientsIds.Count; i++)
            {
                var clientId = this.connectedClientsIds[i];

                if (!this.aliveClientsId.Contains(clientId))
                {
                    result.Add(clientId);
                }
            }

            return result;
        }

        private void UpdateAliveClients()
        {
            if (!this.isRunning)
            {
                return;
            }

            var deadClientsIds = this.GetDeadClientsIds();

            this.connectedClientsIds.RemoveAll(deadClientsIds.Contains);
            this.aliveClientsId.Clear();

            for (int i = 0; i < deadClientsIds.Count; i++)
            {
                var deadClientConnectionId = deadClientsIds[i];
                //better safe than sorry
                try
                {
                    byte error;
                    NetworkTransport.Disconnect(this.genericHostId, deadClientConnectionId, out error);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }

                this.connectedClientsNames.Remove(deadClientConnectionId);
                this.connectedClientsDeviceIds.Remove(deadClientConnectionId);

                if (this.OnClientDisconnected != null)
                {
                    this.OnClientDisconnected(this, new ClientConnectionIdEventArgs(deadClientConnectionId));
                }
            }
        }

        private void OnConnectedClient(NetworkData networkData)
        {
            var connectionId = networkData.ConnectionId;
            var command = NetworkCommandData.From<SendDeviceIdToServerCommand>();
            this.SendClientCommand(connectionId, command);
        }
        
        private void OnClientDisconnect(NetworkData networkData)
        {
            //if disconnected remove from connected clients list
            var connectionId = networkData.ConnectionId;

            try
            {
                this.connectedClientsIds.Remove(connectionId);
                this.connectedClientsNames.Remove(connectionId);
                this.connectedClientsDeviceIds.Remove(connectionId);
            }
            finally
            {
                if (this.OnClientDisconnected != null)
                {
                    this.OnClientDisconnected(this, new ClientConnectionIdEventArgs(connectionId));
                }
            }
        }

        private void FilterCommandLineOptions(NetworkCommandData command)
        {
            if (command.Options.ContainsKey("ConnectionId"))
            {
                command.RemoveOption("ConnectionId");
            }
        }

        private void OnClientSendData(NetworkData receiveNetworkData)
        {
            //if we received data from client
            var connectionId = receiveNetworkData.ConnectionId;
            var message = receiveNetworkData.Message;
            NetworkCommandData commandData = null;

            try
            {
                commandData = NetworkCommandData.Parse(message);
            }
            catch (Exception ex)
            {
                Debug.LogWarning(ex.Message);
            }

            if (commandData != null)
            {
                this.FilterCommandLineOptions(commandData);
                commandData.AddOption("ConnectionId", connectionId.ToString());

                try
                {
                    this.commandsManager.Execute(commandData);
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            }
            else
            {
                if (this.OnReceivedData != null)
                {
                    this.OnReceivedData(this, new DataSentEventArgs(receiveNetworkData.ConnectionId, message));
                }
            }
        }

        public string GetClientUsername(int connectionId)
        {
            if (connectionId == NetworkCommandData.CODE_Option_ClientConnectionId_AI)
            {
                return LanguagesManager.Instance.GetValue("Computer");
            }

            var username = this.connectedClientsNames.FirstOrDefault(ci => ci.Key == connectionId);

            if (username.Equals(new KeyValuePair<int, string>()))
            {
                return string.Empty;
            }
            else
            {
                return username.Value;
            }
        }

        public void StartServer()
        {
            if (this.IsRunning)
            {
                throw new InvalidOperationException("Already running");
            }

            NetworkTransport.Init();
            this.genericHostId = NetworkTransport.AddHost(this.topology, Port, null);
            this.isRunning = true;
        }

        public void StopServer()
        {
            if (!this.IsRunning)
            {
                throw new InvalidOperationException("Not running");
            }

            NetworkTransport.RemoveHost(this.genericHostId);
            NetworkTransport.Shutdown();

            this.isRunning = false;
        }

        public void SetClientUsername(int connectionId, string username)
        {
            if (!this.connectedClientsIds.Contains(connectionId))
            {
                throw new ArgumentException("Client with id " + connectionId + " is not connected");
            }

            this.connectedClientsNames.Add(connectionId, username);

            var connectedClientData = new ConnectedClientData(connectionId, username);
            this.OnClientSetUsername(this, new ConnectedClientDataEventArgs(connectedClientData));
        }

        public void SendClientCommand(int connectionId, NetworkCommandData command)
        {
            this.SendClientMessage(connectionId, command.ToString());
        }

        public void SendClientMessage(int connectionId, string message)
        {
            NetworkTransportUtils.SendMessageAsync(this.genericHostId, connectionId, this.communicationChannel, message, (exception) =>
                {
                    var errorN = exception.ErrorN;
                    var error = (NetworkError)errorN;
                    var errorMessage = NetworkErrorUtils.GetMessage(error);

                    Debug.LogError(errorMessage);
                });
        }

        public void SendAllClientsCommand(NetworkCommandData command)
        {
            this.SendAllClientsMessage(command.ToString());
        }

        public void SendAllClientsCommand(NetworkCommandData command, int exceptConnectionId)
        {
            this.SendAllClientsMessage(command.ToString(), exceptConnectionId);
        }

        public void SendAllClientsMessage(string message)
        {
            for (int i = 0; i < this.connectedClientsIds.Count; i++)
            {
                var clientId = this.connectedClientsIds[i];
                this.SendClientMessage(clientId, message);
            }
        }

        public void SendAllClientsMessage(string message, int exceptConnectionId)
        {
            for (int i = 0; i < this.connectedClientsIds.Count; i++)
            {
                var clientId = this.connectedClientsIds[i];

                if (clientId == exceptConnectionId)
                {
                    continue;
                }

                this.SendClientMessage(clientId, message);
            }
        }

        public void KickPlayer(int connectionId, string message)
        {
            if (!this.IsConnected(connectionId))
            {
                throw new Exception("Client with id " + connectionId + " not connected");
            }

            try
            {
                var commandLine = new NetworkCommandData("ShowNotification");

                commandLine.AddOption("Color", "red");
                commandLine.AddOption("Message", message);

                this.SendClientCommand(connectionId, commandLine);
            }
            catch (NetworkException ex)
            {
                Debug.LogException(ex);
            }

            byte error;
            NetworkTransport.Disconnect(this.genericHostId, connectionId, out error);
        }

        public void KickPlayer(int connectionId)
        {
            this.KickPlayer(connectionId, "Изгонен си от сървъра.");
        }

        public void BanPlayer(int connectionId)
        {
            var deviceId = this.connectedClientsDeviceIds[connectionId];
            this.bannedDeviceIds.Add(deviceId);
            this.KickPlayer(connectionId, "Нямаш право да влизаш във сървъра.");
        }

        public bool IsConnected(int connectionId)
        {
            return this.connectedClientsIds.Contains(connectionId);
        }

        public void Dispose()
        {
            this.OnClientConnected = null;
            this.OnClientDisconnected = null;
            this.OnClientSetUsername = null;
            this.OnReceivedData = null;

            this.updateAliveClientsTimer.Stop();
            this.updateAliveClientsTimer.Dispose();
            this.updateAliveClientsTimer = null;

            instance = null;
        }

        #region DEBUG_MENU

        public bool ShowDebugMenu;

        void OnGUI()
        {
            if (!this.ShowDebugMenu)
            {
                return;
            }

            GUI.Box(new Rect(0, 0, 315, 300), "ServerNetworkManager");

            var connectedPlayersRect = new Rect(5, 30, 150, 30);
            var banRandomPlayerButtonRect = new Rect(5, 55, 145, 30);
            var kickRandomPlayerButtonRect = new Rect(160, 55, 145, 30);

            var banRandomClientButton = GUI.Button(banRandomPlayerButtonRect, "Ban Random Client");
            var kickRandomClientButton = GUI.Button(kickRandomPlayerButtonRect, "Kick Random Client");

            GUI.Label(connectedPlayersRect, "Connected players " + this.connectedClientsIds.Count + '/' + this.MaxConnections);

            if (banRandomClientButton)
            {
                var randomClientId = this.connectedClientsIds.GetRandomElement();
                this.BanPlayer(randomClientId);
            }

            if (kickRandomClientButton)
            {
                var randomClientId = this.connectedClientsIds.GetRandomElement();
                this.KickPlayer(randomClientId);
            }

            var allClientsIdsNames = this.connectedClientsNames.ToList();

            for (int i = 0; i < allClientsIdsNames.Count; i++)
            {
                var clientId = allClientsIdsNames[i].Key;
                var clientUsername = allClientsIdsNames[i].Value;
                var width = 100;
                var height = 30;
                var x = connectedPlayersRect.position.x + 5;
                var y = kickRandomPlayerButtonRect.position.y + kickRandomPlayerButtonRect.height + 5 + ((i * height) + 5);
                var rect = new Rect(x, y, width, height);

                GUI.Label(rect, string.Format("{0} {1}", clientId, clientUsername));
            }
        }

        #endregion
    }
}