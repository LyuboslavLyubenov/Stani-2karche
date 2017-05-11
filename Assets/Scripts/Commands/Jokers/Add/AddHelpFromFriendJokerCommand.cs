using HelpFromFriendJoker = Jokers.HelpFromFriendJoker;
using IAskClientQuestionResultRetriever = Interfaces.Network.Jokers.IAskClientQuestionResultRetriever;

namespace Commands.Jokers.Add
{

    using System.Collections.Generic;

    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Interfaces.Controllers;
    
    using Interfaces.Network.NetworkManager;

    using UnityEngine;

    public class AddHelpFromFriendJokerCommand : AddJokerAbstractCommand
    {
        private IJoker joker;

        public AddHelpFromFriendJokerCommand(IAvailableJokersUIController jokersUIController, 
                                             IClientNetworkManager networkManager,
                                             IAskClientQuestionResultRetriever resultRetriever,
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