namespace Assets.Scripts.Commands.EveryBodyVsTheTeacher.Presenter.States.FirstRoundState
{

    using System;
    using System.Collections.Generic;

    using Controllers.EveryBodyVsTheTeacher.Jokers.Election;

    using Extensions;

    using Interfaces.Network.NetworkManager;
    public class PlayerSelectedForKalitkoJokerCommand : INetworkManagerCommand
    {
        private readonly IJokerElectionUIController jokerElectionUiController;

        public PlayerSelectedForKalitkoJokerCommand(IJokerElectionUIController jokerElectionUiController)
        {
            if (jokerElectionUiController == null)
            {
                throw new ArgumentNullException("jokerElectionUiController");
            }

            this.jokerElectionUiController = jokerElectionUiController;
        }

        public void Execute(Dictionary<string, string> commandsOptionsValues)
        {
            this.jokerElectionUiController.AddThumbsUp();
        }
    }
}
