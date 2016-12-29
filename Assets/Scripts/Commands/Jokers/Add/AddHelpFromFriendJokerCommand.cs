using System.Collections.Generic;

using UnityEngine;

//Mediator

namespace Assets.Scripts.Commands.Jokers.Add
{

    using Assets.Scripts.Controllers;
    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Jokers;
    using Assets.Scripts.Network;
    using Assets.Scripts.Network.NetworkManagers;

    public class AddHelpFromFriendJokerCommand : AddJokerAbstractCommand
    {
        IJoker joker;

        public AddHelpFromFriendJokerCommand(AvailableJokersUIController jokersUIController, 
                                             ClientNetworkManager networkManager,
                                             GameObject callAFriendUI, 
                                             GameObject friendAnswerUI, 
                                             GameObject waitingToAnswerUI, 
                                             GameObject loadingUI)
            : base(jokersUIController)
        {
            this.joker = new HelpFromFriendJoker(networkManager, callAFriendUI, friendAnswerUI, waitingToAnswerUI, loadingUI);
        }

        public override void Execute(Dictionary<string, string> commandsOptionsValues)
        {
            base.AddJoker(this.joker);
        }
    }

}