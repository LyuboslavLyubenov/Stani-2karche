//Mediator

namespace EventArgs
{

    using System;
    using System.Collections.Generic;

    using EventArgs = System.EventArgs;

    public class DummyCommandReceivedDataEventArgs : EventArgs
    {
        public Dictionary<string, string> CommandsOptionsValues
        {
            get;
            private set;
        }

        public DummyCommandReceivedDataEventArgs(Dictionary<string, string> commandsOptionsValues)
        {
            if (commandsOptionsValues == null)
            {
                throw new ArgumentNullException("commandsOptionsValues");
            }
            
            this.CommandsOptionsValues = commandsOptionsValues;
        }
    }

}