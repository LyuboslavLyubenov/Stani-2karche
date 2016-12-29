namespace Assets.Scripts.Commands.Jokers.Add
{
    using System.Collections.Generic;

    using Controllers;
    using Interfaces;
    using Scripts.Jokers;
    using Network.NetworkManagers;

    public class AddDisableRandomAnswersJokerCommand : AddJokerAbstractCommand
    {
        private IJoker joker;

        public AddDisableRandomAnswersJokerCommand(AvailableJokersUIController availableJokersUIController, ClientNetworkManager networkManager, QuestionUIController questionUIController)
            : base(availableJokersUIController)
        {
            this.joker = new DisableRandomAnswersJoker(networkManager, questionUIController);
        }

        public override void Execute(Dictionary<string, string> commandsOptionsValues)
        {
            base.AddJoker(this.joker);
        }
    }
}