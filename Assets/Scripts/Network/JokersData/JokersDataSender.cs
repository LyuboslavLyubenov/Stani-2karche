namespace Network
{
    using System;

    using Commands;

    using EventArgs;

    using Interfaces.Network.NetworkManager;

    public class JokersDataSender
    {
        private readonly int receiverConnectionId;
        private readonly IServerNetworkManager networkManager;

        public JokersDataSender(JokersData jokersData, int receiverConnectionId, IServerNetworkManager networkManager)
        {
            if (jokersData == null)
            {
                throw new ArgumentNullException("jokersData");
            }

            if (receiverConnectionId <= 0)
            {
                throw new ArgumentOutOfRangeException("receiverConnectionId");
            }

            if (networkManager == null)
            {
                throw new ArgumentNullException("networkManager");
            }
            
            this.receiverConnectionId = receiverConnectionId;
            this.networkManager = networkManager;

            jokersData.OnAddedJoker += this.OnAddedJoker;
            jokersData.OnRemovedJoker += this.OnRemovedJoker;
        }
        
        private void OnAddedJoker(object sender, JokerTypeEventArgs args)
        {
            var commandName = "Add" + args.JokerType.Name;
            var command = new NetworkCommandData(commandName);
            this.networkManager.SendClientCommand(this.receiverConnectionId, command);
        }

        private void OnRemovedJoker(object sender, JokerTypeEventArgs args)
        {
            var commandName = "Remove" + args.JokerType.Name;
            var command = new NetworkCommandData(commandName);
            this.networkManager.SendClientCommand(this.receiverConnectionId, command);
        }
    }

}