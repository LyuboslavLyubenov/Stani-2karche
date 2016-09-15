using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Timers;
using System.Collections;

public class ReceiveMainPlayerSelectedHelpFromFriendJokerCommand : INetworkManagerCommand
{
    MainPlayerData mainPlayerData;
    HelpFromFriendJokerRouter jokerServerRouter;
    int timeToAnswerInSeconds;

    Type helpFromFriendJokerType;

    public ReceiveMainPlayerSelectedHelpFromFriendJokerCommand(MainPlayerData mainPlayerData, HelpFromFriendJokerRouter jokerServerRouter, int timeToAnswerInSeconds)
    {
        if (mainPlayerData == null)
        {
            throw new ArgumentNullException("mainPlayerData");
        }

        if (jokerServerRouter == null)
        {
            throw new ArgumentNullException("jokerServerRouter");
        }
            
        this.mainPlayerData = mainPlayerData;
        this.jokerServerRouter = jokerServerRouter;
        this.timeToAnswerInSeconds = timeToAnswerInSeconds;
        this.helpFromFriendJokerType = typeof(HelpFromFriendJoker);
    }

    public void Execute(Dictionary<string, string> commandsOptionsValues)
    {
        var senderConnectionId = int.Parse(commandsOptionsValues["ConnectionId"]);
        var sendClientId = int.Parse(commandsOptionsValues["SendClientId"]);

        if (!mainPlayerData.JokersData.AvailableJokers.Contains(helpFromFriendJokerType))
        {
            return;
        }

        mainPlayerData.JokersData.RemoveJoker(helpFromFriendJokerType);
        jokerServerRouter.Activate(timeToAnswerInSeconds, senderConnectionId, sendClientId);
    }
}
