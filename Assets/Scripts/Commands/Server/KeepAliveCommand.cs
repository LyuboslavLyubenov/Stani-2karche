namespace Commands.Server
{

    using System.Collections.Generic;

    using Interfaces.Network.NetworkManager;

    public class KeepAliveCommand : INetworkManagerCommand
    {
        private ICollection<int> aliveClientsIds;

        public KeepAliveCommand(ICollection<int> aliveClientsIds)
        {
            if (aliveClientsIds == null)
            {
                throw new System.ArgumentNullException("aliveClientsIds");
            }

            this.aliveClientsIds = aliveClientsIds;
        }

        public void Execute(Dictionary<string, string> commandsOptionsValues)
        {
            var connectionId = int.Parse(commandsOptionsValues["ConnectionId"]);
            this.aliveClientsIds.Add(connectionId);
        }
    }
}