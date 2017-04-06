using IOneTimeExecuteCommand = Interfaces.Network.NetworkManager.IOneTimeExecuteCommand;

namespace Assets.Scripts.Commands.Jokers
{
    using System;
    using System.Collections.Generic;
    
    using Controllers.Jokers;
    
    public class KalitkoJokerResultCommand : IOneTimeExecuteCommand
    {
        private readonly KalitkoJokerContainerUIController kalitkoJokerContainerUiController;

        public EventHandler OnFinishedExecution
        {
            get; set;
        }

        public bool FinishedExecution
        {
            get; private set;
        }

        public KalitkoJokerResultCommand(KalitkoJokerContainerUIController kalitkoJokerContainerUiController)
        {
            if (kalitkoJokerContainerUiController == null)
            {
                throw new ArgumentNullException("kalitkoJokerContainerUiController");
            }

            this.kalitkoJokerContainerUiController = kalitkoJokerContainerUiController;
        }

        public void Execute(Dictionary<string, string> commandsOptionsValues)
        {
            if (!commandsOptionsValues.ContainsKey("Answer"))
            {
                this.kalitkoJokerContainerUiController.ShowNothing();
            }
            else
            {
                this.kalitkoJokerContainerUiController.ShowAnswer(commandsOptionsValues["Answer"]);
            }

            this.FinishedExecution = true;
            this.OnFinishedExecution(this, EventArgs.Empty);
        }
    }
}
