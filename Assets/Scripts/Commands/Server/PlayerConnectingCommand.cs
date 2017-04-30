namespace Commands.Server
{

    using System;
    using System.Collections.Generic;

    using Interfaces.Network.NetworkManager;

    public class PlayerConnectingCommand : INetworkManagerCommand
    {
        public delegate void PlayerConnectingDelegate(int connectionId);

        private PlayerConnectingDelegate onPlayerConnecting;

        public PlayerConnectingCommand(PlayerConnectingDelegate onPlayerConnecting)
        {
            if (onPlayerConnecting == null)
            {
                throw new ArgumentNullException("onPlayerConnecting");
            }

            this.onPlayerConnecting = onPlayerConnecting;
        }

        public void Execute(Dictionary<string, string> commandsOptionsValues)
        {
            var connectionId = int.Parse(commandsOptionsValues["ConnectionId"]);
            this.onPlayerConnecting(connectionId);
        }
    }

}