using System;
using System.Linq;

namespace Assets.Scripts.Network
{

    using Assets.Scripts.Commands;
    using Assets.Scripts.EventArgs;

    public class MainPlayerJokersDataSynchronizer
    {
        ServerNetworkManager networkManager;

        MainPlayerData mainPlayerData;

        public MainPlayerJokersDataSynchronizer(ServerNetworkManager networkManager, MainPlayerData mainPlayerData)
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

        void OnMainPlayerConnected(object sender, ClientConnectionDataEventArgs args)
        {
            this.SendAvailableJokersToMainPlayer(args.ConnectionId);
        }

        void OnAddedJoker(object sender, JokerTypeEventArgs args)
        {
            this.SendJokerToPlayer(args.JokerType, this.mainPlayerData.ConnectionId);
        }

        void SendAvailableJokersToMainPlayer(int connectionId)
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

        void SendJokerToPlayer(Type jokerType, int connectionId)
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