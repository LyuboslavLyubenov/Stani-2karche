namespace Assets.Scripts.Network
{
    using System;
    using System.Linq;

    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Interfaces.Network.NetworkManager;

    using Commands;
    using DTOs;
    using EventArgs;
    using NetworkManagers;

    public class MainPlayerJokersDataSynchronizer
    {
        private IServerNetworkManager networkManager;

        private MainPlayerData mainPlayerData;

        public MainPlayerJokersDataSynchronizer(IServerNetworkManager networkManager, MainPlayerData mainPlayerData)
        {
            if (networkManager == null)
            {
                throw new ArgumentNullException("networkManager");
            }

            if (mainPlayerData == null)
            {
                throw new ArgumentNullException("mainPlayerData");
            }
            
            this.networkManager = networkManager;
            this.mainPlayerData = mainPlayerData;

            mainPlayerData.OnConnected += this.OnMainPlayerConnected;
            mainPlayerData.JokersData.OnAddedJoker += this.OnAddedJoker;
        }

        private void OnMainPlayerConnected(object sender, ClientConnectionDataEventArgs args)
        {
            this.SendAvailableJokersToMainPlayer(args.ConnectionId);
        }

        private void OnAddedJoker(object sender, JokerTypeEventArgs args)
        {
            this.SendJokerToPlayer(args.JokerType, this.mainPlayerData.ConnectionId);
        }

        private void SendAvailableJokersToMainPlayer(int connectionId)
        {
            if (!this.mainPlayerData.IsConnected)
            {
                return;
            }

            var jokers = this.mainPlayerData.JokersData.AvailableJokers.ToArray();

            for (int i = 0; i < jokers.Length; i++)
            {
                var joker = jokers[i];
                this.SendJokerToPlayer(joker, connectionId);
            }
        }

        private void SendJokerToPlayer(Type jokerType, int connectionId)
        {
            if (!this.mainPlayerData.IsConnected)
            {
                return;
            }

            var jokerName = jokerType.Name;
            var addJokerCommand = new NetworkCommandData("Add" + jokerName);

            this.networkManager.SendClientCommand(connectionId, addJokerCommand);   
        }
    }

}