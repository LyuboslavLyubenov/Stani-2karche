namespace Commands.EveryBodyVsTheTeacher.PlayersConnectingState
{

    using System;
    using System.Collections.Generic;

    using Interfaces.Network.NetworkManager;

    public class EveryBodyRequestedGameStartCommand : INetworkManagerCommand, INetworkOperationExecutedCallback
    {
        public EventHandler OnExecuted
        {
            get; set;
        }

        public void Execute(Dictionary<string, string> commandsOptionsValues)
        {
            this.OnExecuted(this, EventArgs.Empty);
        }
    }
}