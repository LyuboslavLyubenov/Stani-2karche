using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public class ReceivedSelectedFifthyFifthyChanceJokerCommand : INetworkManagerCommand
{
    MainPlayerData mainPlayerData;
    DisableRandomAnswersJokerRouter jokerRouter;
    ServerNetworkManager networkManager;
    int answersToDisableCount;
    Type jokerType;

    public ReceivedSelectedFifthyFifthyChanceJokerCommand(
        MainPlayerData mainPlayerData, 
        DisableRandomAnswersJokerRouter jokerRouter, 
        ServerNetworkManager networkManager,
        int answersToDisableCount
    )
    {
        if (mainPlayerData == null)
        {
            throw new ArgumentNullException("mainPlayerData");   
        }
        if (jokerRouter == null)
        {
            throw new ArgumentNullException("jokerRouter");
        }
            
        if (networkManager == null)
        {
            throw new ArgumentNullException("networkManager");
        }
            
        this.mainPlayerData = mainPlayerData;
        this.jokerRouter = jokerRouter;
        this.networkManager = networkManager;
        this.answersToDisableCount = answersToDisableCount;
        this.jokerType = typeof(DisableRandomAnswersJoker);
    }

    public void Execute(Dictionary<string, string> commandsOptionsValues)
    {
        var senderConnectionId = int.Parse(commandsOptionsValues["ConnectionId"]);

        if (!mainPlayerData.JokersData.AvailableJokers.Contains(jokerType))
        {
            return;
        }

        mainPlayerData.JokersData.RemoveJoker(jokerType);
        jokerRouter.Activate(answersToDisableCount, mainPlayerData, networkManager);
    }
}