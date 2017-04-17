using AskAudienceJoker = Jokers.AskAudienceJoker;

namespace Commands.Jokers.Add
{

    using System.Collections.Generic;

    using Assets.Scripts.Interfaces;

    using Controllers;

    using Interfaces;
    using Interfaces.Network.Jokers;

    using UnityEngine;

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
