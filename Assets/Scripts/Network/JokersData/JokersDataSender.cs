namespace Network
{
    using System;

    using Commands;

    using EventArgs;

    using Interfaces.Network.NetworkManager;

    public class JokersDataSender
    {
        protected int receiverConnectionId;

        private readonly IServerNetworkManager networkManager;

        public JokersDataSender(JokersData jokersData, int receiverConnectionId, IServerNetworkManager networkManager)
        {
            if (jokersData == null)
            {
                throw new ArgumentNullException("jokersData");
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

        public JokersDataSender(JokersData jokersData, IServerNetworkManager networkManager) : 
            this(jokersData, 0, networkManager)
        {
        }
        
        protected virtual void OnAddedJoker(object sender, JokerTypeEventArgs args)
        {
            var commandName = "Add" + args.JokerType.Name;
            var command = new NetworkCommandData(commandName);
            this.networkManager.SendClientCommand(this.receiverConnectionId, command);
        }

        protected virtual void OnRemovedJoker(object sender, JokerTypeEventArgs args)
        {
            var commandName = "Remove" + args.JokerType.Name;
            var command = new NetworkCommandData(commandName);
            this.networkManager.SendClientCommand(this.receiverConnectionId, command);
        }
    }
}