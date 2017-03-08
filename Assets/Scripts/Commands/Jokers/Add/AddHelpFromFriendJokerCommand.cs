namespace Assets.Scripts.Commands.Jokers.Add
{
    using System.Collections.Generic;

    using Assets.Scripts.Interfaces.Network.Jokers;
    using Assets.Scripts.Interfaces.Network.NetworkManager;

    using UnityEngine;

    using Controllers;
    using Interfaces;
    using Scripts.Jokers;
    using Network.NetworkManagers;

    public class AddHelpFromFriendJokerCommand : AddJokerAbstractCommand
    {
        private IJoker joker;

        public AddHelpFromFriendJokerCommand(AvailableJokersUIController jokersUIController, 
                                             IClientNetworkManager networkManager,
                                             IAskPlayerQuestionResultRetriever resultRetriever,
                                             GameObject callAFriendUI, 
                                             GameObject friendAnswerUI, 
                                             GameObject waitingToAnswerUI, 
                                             GameObject loadingUI)
            : base(jokersUIController)
        {
            this.joker = new HelpFromFriendJoker(networkManager, resultRetriever, callAFriendUI, friendAnswerUI, waitingToAnswerUI, loadingUI);
        }

        public override void Execute(Dictionary<string, string> commandsOptionsValues)
        {
            base.AddJoker(this.joker);
        }
    }

}