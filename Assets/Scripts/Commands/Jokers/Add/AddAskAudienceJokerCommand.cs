namespace Assets.Scripts.Commands.Jokers.Add
{
    using System.Collections.Generic;

    using Assets.Scripts.Interfaces.Network.Jokers;

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
            IAnswerPollResultRetriever pollResultRetriever,
            GameObject waitingToAnswerUI, 
            GameObject audienceAnswerUI, 
            GameObject loadingUI)
            : base(availableJokersUIController)
        {
            this.joker = new AskAudienceJoker(pollResultRetriever, waitingToAnswerUI, audienceAnswerUI, loadingUI);
        }

        public override void Execute(Dictionary<string, string> commandsOptionsValues)
        {
            base.AddJoker(this.joker);
        }
    }
}
