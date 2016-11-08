using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class ReceivedBasicExamGameEndCommand : INetworkManagerCommand
{
    const int LoadLeaderboardTimeoutInSeconds = 10;

    GameObject endGameUI;
    GameObject leaderboardUI;

    public ReceivedBasicExamGameEndCommand(GameObject endGameUI, GameObject leaderboardUI)
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
        this.leaderboardUI = leaderboardUI;
    }

    public void Execute(Dictionary<string, string> commandsOptionsValues)
    {
        var mark = int.Parse(commandsOptionsValues["Mark"]);

        var endGameUIController = endGameUI.GetComponent<EndGameUIController>();
        endGameUI.SetActive(true);
        endGameUIController.SetMark(mark);

        var leaderboardUIController = leaderboardUI.GetComponent<LeaderboardUIController>();
        var leaderboardReceiver = leaderboardUI.GetComponent<LeaderboardReceiver>();

        leaderboardReceiver.Receive(
            (scores) => leaderboardUIController.Populate(scores), 
            () => Debug.LogError("Load leaderboard timeout"), 
            LoadLeaderboardTimeoutInSeconds);
    }
}