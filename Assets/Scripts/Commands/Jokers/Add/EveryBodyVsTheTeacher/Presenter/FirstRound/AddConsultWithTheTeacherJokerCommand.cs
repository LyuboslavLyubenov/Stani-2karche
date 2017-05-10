using AddJokerAbstractCommand = Commands.Jokers.Add.AddJokerAbstractCommand;
using DisableRandomAnswersJoker = Jokers.DisableRandomAnswersJoker;
using IClientNetworkManager = Interfaces.Network.NetworkManager.IClientNetworkManager;
using IQuestionUIController = Interfaces.Controllers.IQuestionUIController;

namespace Assets.Scripts.Commands.Jokers.Add.EveryBodyVsTheTeacher.Presenter
{
    using System.Collections.Generic;

    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Interfaces.Controllers;

    public class AddConsultWithTheTeacherJokerCommand : AddJokerAbstractCommand
    {
        private readonly IJoker joker;

        public AddConsultWithTheTeacherJokerCommand(
            IAvailableJokersUIController jokersUIController, 
            IClientNetworkManager networkManager, 
            IQuestionUIController questionUIController)
            : base(jokersUIController)
        {
            this.joker = new DisableRandomAnswersJoker(networkManager, questionUIController);
        }

        public override void Execute(Dictionary<string, string> commandsOptionsValues)
        {
            base.AddJoker(joker);
        }
    }
}