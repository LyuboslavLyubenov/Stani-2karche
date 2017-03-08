namespace Assets.Scripts.Commands.Jokers.Add
{
    using System.Collections.Generic;

    using Assets.Scripts.Interfaces.Network.NetworkManager;

    using Controllers;
    using Interfaces;
    using Scripts.Jokers;
    using Network.NetworkManagers;

    public class AddDisableRandomAnswersJokerCommand : AddJokerAbstractCommand
    {
        private readonly IJoker joker;

        public AddDisableRandomAnswersJokerCommand(AvailableJokersUIController availableJokersUIController, IClientNetworkManager networkManager, IQuestionUIController questionUIController)
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