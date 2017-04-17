using DisableRandomAnswersJoker = Jokers.DisableRandomAnswersJoker;

namespace Commands.Jokers.Add
{

    using System.Collections.Generic;

    using Assets.Scripts.Interfaces;

    using Controllers;

    using Interfaces;
    using Interfaces.Controllers;
    using Interfaces.Network.NetworkManager;

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