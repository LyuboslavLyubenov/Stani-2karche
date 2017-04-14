using MainPlayerData = Network.MainPlayerData;

namespace Commands.Server
{

    using System;
    using System.Collections.Generic;

    using Interfaces.Network.NetworkManager;

    using EventArgs = System.EventArgs;

    public class SurrenderBasicExamOneTimeCommand : IOneTimeExecuteCommand
    {
        public bool FinishedExecution
        {
            get;
            private set;
        }

        public EventHandler OnFinishedExecution
        {
            get;
            set;
        }

        private MainPlayerData mainPlayerData;

        private Action onSurrender;

        public SurrenderBasicExamOneTimeCommand(MainPlayerData mainPlayerData, Action onSurrender)
        {
            if (mainPlayerData == null)
            {
                throw new ArgumentNullException("mainPlayerData");
            }
            
            if (onSurrender == null)
            {
                throw new ArgumentNullException("onSurrender");
            }

            this.mainPlayerData = mainPlayerData;    
            this.onSurrender = onSurrender;
        }

        public void Execute(Dictionary<string, string> commandsOptionsValues)
        {
            var connectionId = int.Parse(commandsOptionsValues["ConnectionId"]);

            if (!this.mainPlayerData.IsConnected || this.mainPlayerData.ConnectionId != connectionId)
            {
                return;   
            }

            this.onSurrender();

            this.FinishedExecution = true;

            if (this.OnFinishedExecution != null)
            {
                this.OnFinishedExecution(this, EventArgs.Empty);    
            }
        }
    }

}