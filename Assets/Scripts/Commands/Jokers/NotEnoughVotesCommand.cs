namespace Assets.Scripts.Commands.Jokers
{

    using System;
    using System.Collections.Generic;

    using Assets.Scripts.Interfaces.Network.NetworkManager;
    public class NotEnoughVotesForJokerCommand : INetworkManagerCommand, INetworkOperationExecutedCallback
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
