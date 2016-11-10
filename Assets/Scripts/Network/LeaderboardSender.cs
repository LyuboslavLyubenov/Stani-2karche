using System;
using System.Collections.Generic;
using UnityEngine;

public class LeaderboardSender : ExtendedMonoBehaviour
{
    public ServerNetworkManager NetworkManager;
    public LeaderboardSerializer LeaderboardSerializer;



    void Start()
    {
        CoroutineUtils.WaitForFrames(0, () => Initialize());
    }

    void Initialize()
    {
        //tODO FIX TIMEOUT
        var sendLeaderboardEntitiesCommand = new DummyCommand();

        sendLeaderboardEntitiesCommand.OnExecuted += (sender, args) =>
        {
            var connectionId = int.Parse(args.CommandsOptionsValues["ConnectionId"]);
            StartSendingLeaderboardEntities(connectionId);
        };
        
        NetworkManager.CommandsManager.AddCommand("SendLeaderboardEntities", sendLeaderboardEntitiesCommand);   
    }

    void StartSendingLeaderboardEntities(int connectionId)
    {
        var allPlayersData = LeaderboardSerializer.Leaderboard;

        for (int i = 0; i < allPlayersData.Count; i++)
        {
            var playerScore = allPlayersData[i];
            var playerScoreSer = new PlayerScore_Serializable(playerScore);
            var json = JsonUtility.ToJson(playerScoreSer);
            var sendEntityCommand = new NetworkCommandData("LeaderboardEntity");
            sendEntityCommand.AddOption("PlayerScoreJSON", json);

            NetworkManager.SendClientCommand(connectionId, sendEntityCommand);
        }

        var noMoreEntitiesCommand = new NetworkCommandData("LeaderboardNoMoreEntities");
        NetworkManager.SendClientCommand(connectionId, noMoreEntitiesCommand);
    }

}