using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class ClientBasicExamGameEndCommand : INetworkManagerCommand
{
    GameObject endGameUI;
    GameObject leaderboardUI;

    EndGameUIController endGameUIController;
    LeaderboardUIController leaderboardUIController;

    public ClientBasicExamGameEndCommand(GameObject endGameUI, GameObject leaderboardUI)
    {
        if (endGameUI == null)
        {
            throw new ArgumentNullException("endGameUI");
        }

        if (leaderboardUI == null)
        {
            throw new ArgumentNullException("leaderboardUI");
        }

        this.endGameUI = endGameUI;
        this.endGameUIController = endGameUI.GetComponent<EndGameUIController>();
        this.leaderboardUI = leaderboardUI;
        this.leaderboardUIController = leaderboardUI.GetComponent<LeaderboardUIController>();
    }

    public void Execute(Dictionary<string, string> commandsOptionsValues)
    {
        var mark = int.Parse(commandsOptionsValues["Mark"]);
        var leaderboardDataJSON = commandsOptionsValues["LeaderboardDataJSON"];
        var leaderboardData = JsonUtility.FromJson<LeaderboardData_Serializable>(leaderboardDataJSON);
        var playersScores = leaderboardData.PlayerScore.Select(d => PlayerScore.CreateFrom(d)).ToArray();

        endGameUI.SetActive(true);
        endGameUIController.SetMark(mark);

        leaderboardUI.SetActive(true);
        leaderboardUIController.Populate(playersScores);
    }
}