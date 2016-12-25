using System;
using System.Collections.Generic;

using UnityEngine;

namespace Assets.Scripts.Commands.Client
{

    using Assets.Scripts.Controllers;
    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Network;

    using Debug = UnityEngine.Debug;

    public class BasicExamGameEndCommand : INetworkManagerCommand
    {
        const int LoadLeaderboardTimeoutInSeconds = 10;

        GameObject endGameUI;
        GameObject leaderboardUI;

        public BasicExamGameEndCommand(GameObject endGameUI, GameObject leaderboardUI)
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

            var endGameUIController = this.endGameUI.GetComponent<EndGameUIController>();
            this.endGameUI.SetActive(true);
            endGameUIController.SetMark(mark);

            var leaderboardUIController = this.leaderboardUI.GetComponent<LeaderboardUIController>();
            var leaderboardReceiver = this.leaderboardUI.GetComponent<LeaderboardReceiver>();

            leaderboardReceiver.Receive(
                (scores) => leaderboardUIController.Populate(scores), 
                () => Debug.LogError("Load leaderboard timeout"), 
                LoadLeaderboardTimeoutInSeconds);
        }
    }

}