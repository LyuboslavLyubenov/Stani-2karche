using AddJokerAbstractCommand = Commands.Jokers.Add.AddJokerAbstractCommand;
using IClientNetworkManager = Interfaces.Network.NetworkManager.IClientNetworkManager;

namespace Assets.Scripts.Commands.Jokers.Add.EveryBodyVsTheTeacher.Presenter
{

    using System.Collections.Generic;

    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Interfaces.Controllers;
    using Assets.Scripts.Jokers.EveryBodyVsTheTeacher.Presenter;

    using UnityEngine;

    public class AddTrustRandomPersonJokerCommand : AddJokerAbstractCommand
    {
        private IJoker joker;

        public AddTrustRandomPersonJokerCommand(
            IAvailableJokersUIController jokersUIController,
            IClientNetworkManager networkManager,
            GameObject loadingUI,
            GameObject secondsRemainingUI,
            ISecondsRemainingUIController secondsRemainingUIController,
            GameObject notReceivedAnswerUI,
            GameObject playerAnswerUI,
            IPlayerAnswerUIController playerAnswerUIController)
            : base(jokersUIController)
        {
            this.joker = 
                new TrustRandomPersonJoker(
                    networkManager, 
                    loadingUI, 
                    secondsRemainingUI, 
                    secondsRemainingUIController, 
                    notReceivedAnswerUI,
                    playerAnswerUI,
                    playerAnswerUIController);
        }

        public override void Execute(Dictionary<string, string> commandsOptionsValues)
        {
            this.AddJoker(this.joker);
        }
    }
}
