using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

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
        base.AddJoker(joker);
    }
}
