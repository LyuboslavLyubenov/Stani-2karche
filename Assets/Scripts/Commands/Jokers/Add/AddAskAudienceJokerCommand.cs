using System.Collections.Generic;

using UnityEngine;

namespace Assets.Scripts.Commands.Jokers.Add
{

    using Assets.Scripts.Controllers;
    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Jokers;
    using Assets.Scripts.Network;
    using Assets.Scripts.Notifications;

    public class AddAskAudienceJokerCommand : AddJokerAbstractCommand
    {
        IJoker joker;

        public AddAskAudienceJokerCommand(
            AvailableJokersUIController availableJokersUIController, 
            ClientNetworkManager networkManager, 
            GameObject waitingToAnswerUI, 
            GameObject audienceAnswerUI, 
            GameObject loadingUI,
            NotificationsServiceController notificationService)
            : base(availableJokersUIController)
        {
            this.joker = new AskAudienceJoker(networkManager, waitingToAnswerUI, audienceAnswerUI, loadingUI, notificationService);
        }

        public override void Execute(Dictionary<string, string> commandsOptionsValues)
        {
            base.AddJoker(this.joker);
        }
    }

}
