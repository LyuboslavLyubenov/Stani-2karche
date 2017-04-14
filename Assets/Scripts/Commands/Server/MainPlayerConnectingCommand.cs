namespace Commands.Server
{

    using System;
    using System.Collections.Generic;

    using Interfaces.Network.NetworkManager;

    public class MainPlayerConnectingCommand : INetworkManagerCommand
    {
        public delegate void MainPlayerConnectingDelegate(int connectionId);

        private MainPlayerConnectingDelegate onMainPlayerConnecting;

        public MainPlayerConnectingCommand(MainPlayerConnectingDelegate onMainPlayerConnecting)
        {
            if (onMainPlayerConnecting == null)
            {
                throw new ArgumentNullException("onMainPlayerConnecting");
            }

            this.onMainPlayerConnecting = onMainPlayerConnecting;
        }

        public void Execute(Dictionary<string, string> commandsOptionsValues)
        {
            var connectionId = int.Parse(commandsOptionsValues["ConnectionId"]);
            this.onMainPlayerConnecting(connectionId);
        }

    }

}