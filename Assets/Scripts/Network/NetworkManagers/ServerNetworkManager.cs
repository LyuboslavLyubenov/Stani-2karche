namespace Assets.Scripts.Network.NetworkManagers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using Assets.Scripts.Network.Broadcast;

    using Commands;
    using Commands.Server;

    using DTOs;
    using EventArgs;
    using Exceptions;
    using Localization;

    using Utils;
    using Utils.Unity;

    using UnityEngine;
    using UnityEngine.Networking;

    using Debug = UnityEngine.Debug;

    public class ServerNetworkManager : ExtendedMonoBehaviour
    {
        const int Port = 7788;
        const float CheckForDeadClientsDelayInSeconds = 5f;

        public bool AutoStart = true;
        //how many clients can be connected to the server
        public int MaxConnections;
        public LANServerOnlineBroadcastService LANServerOnlineBroadcastService;

        public EventHandler<ClientConnectionDataEventArgs> OnClientConnected = delegate
            {
            };

        public EventHandler<DataSentEventArgs> OnReceivedData = delegate
            {
            };

        public EventHandler<ClientConnectionDataEventArgs> OnClientDisconnected = delegate
            {
            };

        public EventHandler<ConnectedClientDataEventArgs> OnClientSetUsername = delegate
            {
            };

        int genericHostId = 0;

        ConnectionConfig connectionConfig = null;
        HostTopology topology = null;

        byte communicationChannel = 0;

        bool isRunning = false;
        //Id of all connected clients
        List<int> connectedClientsIds = new List<int>();
        List<int> bannedConnections = new List<int>();
    
        HashSet<int> aliveClientsId = new HashSet<int>();
        Dictionary<int, string> connectedClientsNames = new Dictionary<int, string>();

        CommandsManager commandsManager = new CommandsManager();

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

        public string[] GetAllClientsNames()
        {
            return this.connectedClientsNames.Select(c => c.Value).ToArray();
        }

        public CommandsManager CommandsManager
        {
            get
            {
                return this.commandsManager;
            }
        }

        void Start()
        {
            this.ConfigureCommands();
            this.ConfigureServer();

            if (this.AutoStart)
            {
                this.StartServer();    
            }

            if (this.LANServerOnlineBroadcastService != null)
            {
                this.LANServerOnlineBroadcastService.Start();     
            }
       
            this.CoroutineUtils.RepeatEverySeconds(CheckForDeadClientsDelayInSeconds, this.UpdateAliveClients);
            this.StartCoroutine(this.UpdateCoroutine());
        }

        void OnDisable()
        {
            this.StopAllCoroutines();
            this.StopServer();
        }

        void ConfigureServer()
        {
            this.connectionConfig = new ConnectionConfig();
            this.communicationChannel = this.connectionConfig.AddChannel(QosType.ReliableSequenced);
            this.topology = new HostTopology(this.connectionConfig, this.MaxConnections);
        }

        void ConfigureCommands()
        {
            this.commandsManager.AddCommand(new SetUsernameCommand(this));
            this.commandsManager.AddCommand(new KeepAliveCommand(this.aliveClientsId));
            this.commandsManager.AddCommand(new ServerSendConnectedClientsCountCommand(this));
            this.commandsManager.AddCommand(new ServerSendConnectedClientsIdsNamesCommand(this, this.connectedClientsNames));
        }

        IEnumerator UpdateCoroutine()
        {
            while (true)
            {
                if (this.isRunning)
                {
                    this.UpdateServer();
                }

                yield return null;
            }
        }

        void UpdateServer()
        {
            NetworkTransportUtils.ReceiveMessageAsync(this.ReceivedDataFromClientAsync, (exception) =>
                {
                    var error = (NetworkError)exception.ErrorN;
                    var errorMessage = NetworkErrorUtils.GetMessage(error);

                    Debug.LogError(errorMessage);
                });
        }

        void ReceivedDataFromClientAsync(NetworkData networkData)
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

        IList<int> GetDeadClientsIds()
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

        void UpdateAliveClients()
        {
            if (!this.isRunning)
            {
                return;
            }

            var deadClientsIds = this.GetDeadClientsIds();

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
                    Debug.Log(e.Message);
                }

                this.connectedClientsNames.Remove(deadClientConnectionId);

                this.OnClientDisconnected(this, new ClientConnectionDataEventArgs(deadClientConnectionId));
            }

            this.connectedClientsIds.RemoveAll(deadClientsIds.Contains);
            this.aliveClientsId.Clear();
        }

        void OnConnectedClient(NetworkData networkData)
        {
            var connectionId = networkData.ConnectionId;
            this.connectedClientsIds.Add(connectionId);

            var isBanned = this.bannedConnections.IndexOf(connectionId) > -1;

            if (isBanned)
            {
                this.KickPlayer(connectionId, "Забранено ти е да влизаш във този сървър.");
                return;
            }

            var commandData = new NetworkCommandData("AllowedToConnect");
            this.SendClientCommand(connectionId, commandData);

            this.aliveClientsId.Add(connectionId);

            if (this.OnClientConnected != null)
            {
                this.OnClientConnected(this, new ClientConnectionDataEventArgs(connectionId));    
            }
        }

        void OnClientDisconnect(NetworkData networkData)
        {
            //if disconnected remove from connected clients list
            var connectionId = networkData.ConnectionId;

            try
            {
                this.connectedClientsIds.Remove(connectionId);
                this.connectedClientsNames.Remove(connectionId);    
            }
            catch
            {
            
            }

            if (this.OnClientDisconnected != null)
            {
                this.OnClientDisconnected(this, new ClientConnectionDataEventArgs(connectionId));    
            }
        }

        void FilterCommandLineOptions(NetworkCommandData command)
        {
            var optionsKeyValue = command.Options.ToArray();

            for (int i = 0; i < optionsKeyValue.Length; i++)
            {
                var optionKeyValue = optionsKeyValue[i];
                var key = optionKeyValue.Key;

                if (key == "ConnectionId")
                {
                    command.Options.Remove(key);
                }
            }
        }

        void OnClientSendData(NetworkData receiveNetworkData)
        {
            //if we received data from client
            var connectionId = receiveNetworkData.ConnectionId;
            var message = receiveNetworkData.Message;
            NetworkCommandData commmandData = null;

            try
            {
                commmandData = NetworkCommandData.Parse(message);
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
            }

            if (commmandData != null)
            {
                this.FilterCommandLineOptions(commmandData);
                commmandData.AddOption("ConnectionId", connectionId.ToString());

                try
                {
                    this.commandsManager.Execute(commmandData);    
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            }
            else
            {
                var username = this.GetClientUsername(connectionId); 

                if (this.OnReceivedData != null)
                {
                    this.OnReceivedData(this, new DataSentEventArgs(receiveNetworkData.ConnectionId, username, message));    
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
                return "Клиент номер " + connectionId;
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
                Debug.Log(ex.Message);
            }
            
            this.CoroutineUtils.WaitForFrames(1, () =>
                {
                    byte error;
                    NetworkTransport.Disconnect(this.genericHostId, connectionId, out error);
                });
        }

        public void KickPlayer(int connectionId)
        {
            this.KickPlayer(connectionId, "Изгонен си от сървъра.");
        }

        public void BanPlayer(int connectionId)
        {
            if (this.bannedConnections.Contains(connectionId))
            {
                return;
            }

            this.bannedConnections.Add(connectionId);
            this.KickPlayer(connectionId, "Нямаш право да влизаш във сървъра.");
        }

        public bool IsConnected(int connectionId)
        {
            return this.connectedClientsIds.Contains(connectionId);
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
