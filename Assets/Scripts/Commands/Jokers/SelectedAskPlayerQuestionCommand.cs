using System.Collections.Generic;
using System;

public class SelectedAskPlayerQuestionCommand : INetworkManagerCommand, INetworkOperationExecutedCallback
{
    public EventHandler OnExecuted
    {
        get;
        set;
    }

    ServerNetworkManager networkManager;

    MainPlayerData mainPlayerData;
    AskPlayerQuestionRouter jokerServerRouter;
    int timeToAnswerInSeconds;

    Type helpFromFriendJokerType;

    public SelectedAskPlayerQuestionCommand(ServerNetworkManager networkManager, MainPlayerData mainPlayerData, AskPlayerQuestionRouter jokerServerRouter, int timeToAnswerInSeconds)
    {
        if (networkManager == null)
        {
            throw new ArgumentNullException("networkManager");
        }
            
        if (mainPlayerData == null)
        {
            throw new ArgumentNullException("mainPlayerData");
        }

        if (jokerServerRouter == null)
        {
            throw new ArgumentNullException("jokerServerRouter");
        }

        this.networkManager = networkManager;
        this.mainPlayerData = mainPlayerData;
        this.jokerServerRouter = jokerServerRouter;
        this.timeToAnswerInSeconds = timeToAnswerInSeconds;
        this.helpFromFriendJokerType = typeof(HelpFromFriendJoker);
    }

    public void Execute(Dictionary<string, string> commandsOptionsValues)
    {
        var senderConnectionId = int.Parse(commandsOptionsValues["ConnectionId"]);
        var sendClientId = int.Parse(commandsOptionsValues["PlayerConnectionId"]);

        if (!mainPlayerData.JokersData.AvailableJokers.Contains(helpFromFriendJokerType) || !networkManager.IsConnected(senderConnectionId))
        {
            return;
        }

        mainPlayerData.JokersData.RemoveJoker(helpFromFriendJokerType);
        jokerServerRouter.Activate(timeToAnswerInSeconds, senderConnectionId, sendClientId);

        if (OnExecuted != null)
        {
            OnExecuted(this, EventArgs.Empty);
        }
    }
}
