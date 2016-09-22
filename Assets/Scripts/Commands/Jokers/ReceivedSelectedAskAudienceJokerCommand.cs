using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public class ReceivedSelectedAskAudienceJokerCommand : INetworkManagerCommand
{
    MainPlayerData mainPlayerData;
    AskAudienceJokerRouter askAudienceJokerRouter;
    ServerNetworkManager networkManager;
    Type askAudienceJokerType;
    int timeToAnswerInSeconds;

    public ReceivedSelectedAskAudienceJokerCommand(MainPlayerData mainPlayerData, AskAudienceJokerRouter askAudienceJokerRouter, ServerNetworkManager networkManager, int timeToAnswerInSeconds)
    {
        if (mainPlayerData == null)
        {
            throw new ArgumentNullException("mainPlayerData");
        }

        if (askAudienceJokerRouter == null)
        {
            throw new ArgumentNullException("askAudienceJokerRouter");
        }

        if (networkManager == null)
        {
            throw new ArgumentNullException("networkManager");
        }
            
        this.mainPlayerData = mainPlayerData;
        this.askAudienceJokerRouter = askAudienceJokerRouter;
        this.networkManager = networkManager;
        this.timeToAnswerInSeconds = timeToAnswerInSeconds;
        this.askAudienceJokerType = typeof(AskAudienceJoker);
    }

    public void Execute(Dictionary<string, string> commandsOptionsValues)
    {
        var senderConnectionId = int.Parse(commandsOptionsValues["ConnectionId"]);

        if (!mainPlayerData.JokersData.AvailableJokers.Contains(askAudienceJokerType))
        {
            return;
        }

        mainPlayerData.JokersData.RemoveJoker(askAudienceJokerType);

        var commandData = new NetworkCommandData("AllowedToActivateAskAudienceJoker");
        networkManager.SendClientCommand(senderConnectionId, commandData);
        askAudienceJokerRouter.Activate(timeToAnswerInSeconds, senderConnectionId, mainPlayerData);
    }
}