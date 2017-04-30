using CommandsManager = Commands.CommandsManager;
using NetworkCommandData = Commands.NetworkCommandData;

namespace Tests.DummyObjects
{

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using DTOs;

    using EventArgs;

    using Interfaces.Network.NetworkManager;

    using UnityEngine;

    public class DummyServerNetworkManager : IServerNetworkManager
    {
        public event EventHandler<ClientConnectionIdEventArgs> OnClientConnected = delegate
            {
            };

        public event EventHandler<DataSentEventArgs> OnReceivedData = delegate
            {
            };

        public event EventHandler<ConnectedClientDataEventArgs> OnClientSetUsername = delegate
            {
            };

        public event EventHandler<ClientConnectionIdEventArgs> OnClientDisconnected = delegate
            {
            };

        public event EventHandler<DataSentEventArgs> OnSentDataToClient = delegate
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

        public int MaxConnections { get; set; }

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

        private DummyServerNetworkManager()
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
            this.SendClientMessage(connectionId, command.ToString());
            //
        }

        public void SendClientMessage(int connectionId, string message)
        {
            this.OnSentDataToClient(this, new DataSentEventArgs(connectionId, message));
        }

        public void SendAllClientsCommand(NetworkCommandData command)
        {
            this.SendAllClientsMessage(command.ToString());
        }

        public void SendAllClientsCommand(NetworkCommandData command, int exceptConnectionId)
        {
            var commandStr = command.ToString();

            for (int i = 0; i < this.ConnectedClientsCount; i++)
            {
                var connectionId = this.ConnectedClientsConnectionId[i];

                if (connectionId == exceptConnectionId)
                {
                    continue;
                }

                this.SendClientMessage(connectionId, commandStr);
            }
        }

        public void SendAllClientsMessage(string message)
        {
            for (int i = 0; i < this.ConnectedClientsCount; i++)
            {
                var connectionId = this.ConnectedClientsConnectionId[i];
                this.SendClientMessage(connectionId, message);
            }
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
            this.OnClientConnected(this, new ClientConnectionIdEventArgs(connectionId));
        }

        public void FakeSetUsernameToPlayer(int connectionId, string username)
        {
            this.SetClientUsername(connectionId, username);
        }

        public void FakeDisconnectPlayer(int connectionId)
        {
            this.connectedClientsConnectionIds.Remove(connectionId);
            this.clientsConnectionIdsNames.Remove(connectionId);

            this.OnClientDisconnected(this, new ClientConnectionIdEventArgs(connectionId));
        }

        public void FakeReceiveMessage(int fromConnectionId, string message)
        {
            var clientUsername = this.GetClientUsername(fromConnectionId);

            NetworkCommandData commandData = null;

            try
            {
                commandData = NetworkCommandData.Parse(message);
            }
            catch
            {
            }

            if (commandData != null)
            {
                commandData.AddOption("ConnectionId", fromConnectionId.ToString());

                try
                {
                    this.CommandsManager.Execute(commandData);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
            else
            {
                this.OnReceivedData(this, new DataSentEventArgs(fromConnectionId, message));
            }
        }
    }
}