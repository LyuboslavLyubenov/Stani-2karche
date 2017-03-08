using System;
using System.Collections.Generic;

//Mediator

namespace Assets.Scripts.Commands
{

    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Interfaces.Network.NetworkManager;

    using EventArgs = System.EventArgs;

    public class DummyOneTimeCommand : IOneTimeExecuteCommand
    {
        public EventHandler OnFinishedExecution
        {
            get;
            set;
        }

        public bool FinishedExecution
        {
            get;
            private set;
        }

        public void Execute(Dictionary<string, string> commandsOptionsValues)
        {
            if (this.OnFinishedExecution != null)
            {
                this.OnFinishedExecution(this, EventArgs.Empty);
            }

            this.FinishedExecution = true;
        }
    }

}