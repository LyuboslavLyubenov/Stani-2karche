using System.Collections.Generic;

//Mediator

namespace Assets.Scripts.Commands.Jokers.Add
{

    using Assets.Scripts.Controllers;
    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Jokers;
    using Assets.Scripts.Network;

    public class AddDisableRandomAnswersJokerCommand : AddJokerAbstractCommand
    {
        IJoker joker;

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