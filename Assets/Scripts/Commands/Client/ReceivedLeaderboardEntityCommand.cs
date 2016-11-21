using System;
using System.Collections.Generic;
using UnityEngine;

public class ReceivedLeaderboardEntityCommand : INetworkManagerCommand
{
    ICollection<PlayerScore> playersScores;

    public ReceivedLeaderboardEntityCommand(ICollection<PlayerScore> playersScores)
    {
        if (playersScores == null)
        {
            throw new System.ArgumentNullException("playersScores");
        }
            
        this.playersScores = playersScores;
    }

    public void Execute(Dictionary<string, string> commandsOptionsValues)
    {
        var playerScoreJSON = commandsOptionsValues["PlayerScoreJSON"];
        var playerScoreSer = JsonUtility.FromJson<PlayerScore_Serializable>(playerScoreJSON);
        var playerScore = PlayerScore.CreateFrom(playerScoreSer);
        playersScores.Add(playerScore);
    }
}