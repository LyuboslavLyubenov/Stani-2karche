namespace Assets.Scripts.Commands.Client
{
    using System;

    using Interfaces;
    using Utils;

    public class ConnectedClientsCountCommand : INetworkManagerCommand
    {
        private ValueWrapper<int> serverConnectedClientsCount;

        public ConnectedClientsCountCommand(ValueWrapper<int> serverConnectedClientsCount)
        {
            if (serverConnectedClientsCount == null)
            {
                throw new ArgumentNullException("serverConnectedClientsCount");
            }
            
            this.serverConnectedClientsCount = serverConnectedClientsCount;       
        }

        public void Execute(System.Collections.Generic.Dictionary<string, string> commandsOptionsValues)
        {
            var clientsCount = int.Parse(commandsOptionsValues["ConnectedClientsCount"]);
            this.serverConnectedClientsCount.Value = clientsCount;
        }
    }
}