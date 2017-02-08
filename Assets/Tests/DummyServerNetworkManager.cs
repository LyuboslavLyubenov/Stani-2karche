namespace Assets.Tests
{

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Assets.Scripts.Commands;
    using Assets.Scripts.DTOs;
    using Assets.Scripts.EventArgs;
    using Assets.Scripts.Interfaces;

    public class DummyServerNetworkManager : IServerNetworkManager
    {
        public event EventHandler<ClientConnectionDataEventArgs> OnClientConnected = delegate
            {
            };

        public event EventHandler<DataSentEventArgs> OnReceivedData = delegate
            {
            };

        public event EventHandler<ConnectedClientDataEventArgs> OnClientSetUsername = delegate
            {
            };

        public event EventHandler<ClientConnectionDataEventArgs> OnClientDisconnected = delegate
            {
            };

        private static DummyServerNetworkManager instance;

        public static DummyServerNetworkManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DummyServerNetworkManager();
                }

                return instance;
             }
        }

        public ICommandsManager CommandsManager
        {
            get; private set;
        }

        public bool IsRunning
        {
            get; private set;
        }

        public int ConnectedClientsCount
        {
            get
            {
                return this.connectedClientsConnectionIds.Count;
            }
        }

        public int[] ConnectedClientsConnectionId
        {
            get
            {
                return this.connectedClientsConnectionIds.ToArray();
            }
        }

        private HashSet<int> connectedClientsConnectionIds = new HashSet<int>();
        private Dictionary<int, string> clientsConnectionIdsNames = new Dictionary<int, string>();

        public DummyServerNetworkManager()
        {
            this.CommandsManager = new CommandsManager();
        }

        public string[] GetAllClientsNames()
        {
            return this.clientsConnectionIdsNames.Values.ToArray();
        }

        public string GetClientUsername(int connectionId)
        {
            var username = "";

            if (this.clientsConnectionIdsNames.ContainsKey(connectionId))
            {
                username = this.clientsConnectionIdsNames[connectionId];
            }

            return username;
        }

        public void StartServer()
        {
            this.IsRunning = true;
        }

        public void StopServer()
        {
            this.IsRunning = false;
        }

        public void SetClientUsername(int connectionId, string username)
        {
            this.clientsConnectionIdsNames[connectionId] = username;
            this.OnClientSetUsername(this, new ConnectedClientDataEventArgs(new ConnectedClientData(connectionId, username)));
        }

        public void SendClientCommand(int connectionId, NetworkCommandData command)
        {
            //
        }

        public void SendClientMessage(int connectionId, string message)
        {
            //
        }

        public void SendAllClientsCommand(NetworkCommandData command)
        {
            //
        }

        public void SendAllClientsCommand(NetworkCommandData command, int exceptConnectionId)
        {
            //
        }

        public void SendAllClientsMessage(string message)
        {
            //
        }

        public void KickPlayer(int connectionId, string message)
        {
            this.connectedClientsConnectionIds.Remove(connectionId);
            this.clientsConnectionIdsNames.Remove(connectionId);
        }

        public void KickPlayer(int connectionId)
        {
            this.KickPlayer(connectionId, "");
        }

        public void BanPlayer(int connectionId)
        {
            this.KickPlayer(connectionId, "");
        }

        public bool IsConnected(int connectionId)
        {
            return this.connectedClientsConnectionIds.Contains(connectionId);
        }

        //

        public void FakeConnectPlayer(int connectionId)
        {
            this.connectedClientsConnectionIds.Add(connectionId);            
            this.OnClientConnected(this, new ClientConnectionDataEventArgs(connectionId));
        }

        public void FakeSetUsernameToPlayer(int connectionId, string username)
        {
            this.SetClientUsername(connectionId, username);
        }

        public void FakeDisconnectPlayer(int connectionId)
        {
            this.connectedClientsConnectionIds.Remove(connectionId);
            this.clientsConnectionIdsNames.Remove(connectionId);

            this.OnClientDisconnected(this, new ClientConnectionDataEventArgs(connectionId));
        }

        public void FakeReceiveMessage(int fromConnectionId, string message)
        {
            var clientUsername = this.GetClientUsername(fromConnectionId);
            this.OnReceivedData(this, new DataSentEventArgs(fromConnectionId, clientUsername, message));
        }

    }

}