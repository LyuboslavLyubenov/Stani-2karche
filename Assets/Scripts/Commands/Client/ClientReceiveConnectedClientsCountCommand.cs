namespace Assets.Scripts.Commands.Client
{
    using System;

    using Interfaces;
    using Utils;

    public class ClientReceiveConnectedClientsCountCommand : INetworkManagerCommand
    {
        private ValueWrapper<int> serverConnectedClientsCount;

        public ClientReceiveConnectedClientsCountCommand(ValueWrapper<int> serverConnectedClientsCount)
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