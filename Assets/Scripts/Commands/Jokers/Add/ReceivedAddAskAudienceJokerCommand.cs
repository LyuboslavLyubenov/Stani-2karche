using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ReceivedAddAskAudienceJokerCommand : ReceivedAddJokerAbstractCommand
{
    IJoker joker;

    public ReceivedAddAskAudienceJokerCommand(AvailableJokersUIController availableJokersUIController, ClientNetworkManager networkManager, GameObject waitingToAnswerUI, GameObject audienceAnswerUI, GameObject loadingUI)
        : base(availableJokersUIController)
    {
        this.joker = new AskAudienceJoker(networkManager, waitingToAnswerUI, audienceAnswerUI, loadingUI);
    }

    public override void Execute(Dictionary<string, string> commandsOptionsValues)
    {
        base.AddJoker(joker);
    }
}
