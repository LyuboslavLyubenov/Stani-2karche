namespace Commands.Jokers.JokerElection.KalitkoJoker.Presenter
{

    using System;
    using System.Collections.Generic;

    using Controllers.Jokers;

    using Interfaces.Network.NetworkManager;

    public class AllPlayersSelectedKalitkoJokerCommand : INetworkManagerCommand
    {
        private readonly KalitkoJokerContainerUIController kalitkoJokerContainerUiController;

        public AllPlayersSelectedKalitkoJokerCommand(KalitkoJokerContainerUIController kalitkoJokerContainerUiController)
        {
            if (kalitkoJokerContainerUiController == null)
            {
                throw new ArgumentNullException("kalitkoJokerContainerUiController");
            }

            this.kalitkoJokerContainerUiController = kalitkoJokerContainerUiController;
        }

        public void Execute(Dictionary<string, string> commandsOptionsValues)
        {
            if (commandsOptionsValues.ContainsKey("Answer"))
            {
                this.kalitkoJokerContainerUiController.ShowAnswer(commandsOptionsValues["Answer"]);
            }
            else
            {
                this.kalitkoJokerContainerUiController.ShowNothing();
            }
        }
    }
}
