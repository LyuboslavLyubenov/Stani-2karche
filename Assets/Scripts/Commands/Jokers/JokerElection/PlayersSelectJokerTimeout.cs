using INetworkManagerCommand = Interfaces.Network.NetworkManager.INetworkManagerCommand;
using INetworkOperationExecutedCallback = Interfaces.Network.NetworkManager.INetworkOperationExecutedCallback;
using IOneTimeExecuteCommand = Interfaces.Network.NetworkManager.IOneTimeExecuteCommand;

namespace Commands.Jokers.JokerElection
{

    using System;
    using System.Collections.Generic;

    public class PlayersSelectJokerTimeoutCommand : INetworkManagerCommand, INetworkOperationExecutedCallback
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

    public class PlayersSelectedJokerTimeoutOneTimeCommand : IOneTimeExecuteCommand
    {
        public EventHandler OnFinishedExecution
        {
            get; set;
        }

        public bool FinishedExecution
        {
            get; private set;
        }
        
        public void Execute(Dictionary<string, string> commandsOptionsValues)
        {
            this.FinishedExecution = true;
            this.OnFinishedExecution(this, EventArgs.Empty);
        }
    }
}
