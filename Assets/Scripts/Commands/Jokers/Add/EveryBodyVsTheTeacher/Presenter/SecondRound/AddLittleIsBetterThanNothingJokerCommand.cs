using AddJokerAbstractCommand = Commands.Jokers.Add.AddJokerAbstractCommand;
using IClientNetworkManager = Interfaces.Network.NetworkManager.IClientNetworkManager;
using IQuestionUIController = Interfaces.Controllers.IQuestionUIController;

namespace Assets.Scripts.Commands.Jokers.Add.EveryBodyVsTheTeacher.Presenter.SecondRound
{

    using System.Collections.Generic;

    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Interfaces.Controllers;
    using Assets.Scripts.Jokers;

    public class AddLittleIsBetterThanNothingJokerCommand : AddJokerAbstractCommand
    {
        private IJoker joker;

        public AddLittleIsBetterThanNothingJokerCommand(
            IAvailableJokersUIController jokersUIController, 
            IClientNetworkManager networkManager,
            IQuestionUIController questionUIcontroller)
            : base(jokersUIController)
        {
            this.joker = new LittleIsBetterThanNothingJoker(networkManager, questionUIcontroller);
        }

        public override void Execute(Dictionary<string, string> commandsOptionsValues)
        {
            this.AddJoker(this.joker);
        }
    }
}
