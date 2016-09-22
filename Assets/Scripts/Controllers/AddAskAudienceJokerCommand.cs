using UnityEngine;
using System;
using System.Collections;

//Mediator
using System.Collections.Generic;

public class AddAskAudienceJokerCommand : AddJokerAbstractCommand
{
    IJoker joker;

    public AddAskAudienceJokerCommand(AvailableJokersUIController availableJokersUIController, IGameData gameData, ClientNetworkManager networkManager, GameObject waitingToAnswerUI, GameObject audienceAnswerUI, GameObject loadingUI)
        : base(availableJokersUIController)
    {
        this.joker = new AskAudienceJoker(gameData, networkManager, waitingToAnswerUI, audienceAnswerUI, loadingUI);
    }

    public override void Execute(Dictionary<string, string> commandsOptionsValues)
    {
        base.AddJoker(joker);
    }
}
