namespace Assets.Scripts.Commands.Jokers.Add
{
    using System.Collections.Generic;

    using UnityEngine;

    using Controllers;
    using Interfaces;
    using Scripts.Jokers;
    using Network.NetworkManagers;

    public class AddAskAudienceJokerCommand : AddJokerAbstractCommand
    {
        private IJoker joker;

        public AddAskAudienceJokerCommand(
            AvailableJokersUIController availableJokersUIController, 
            ClientNetworkManager networkManager, 
            GameObject waitingToAnswerUI, 
            GameObject audienceAnswerUI, 
            GameObject loadingUI)
            : base(availableJokersUIController)
        {
            this.joker = new AskAudienceJoker(networkManager, waitingToAnswerUI, audienceAnswerUI, loadingUI);
        }

        public override void Execute(Dictionary<string, string> commandsOptionsValues)
        {
            base.AddJoker(this.joker);
        }
    }
}
