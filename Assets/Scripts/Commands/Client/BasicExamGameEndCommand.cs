namespace Assets.Scripts.Commands.Client
{
    using System;
    using System.Collections.Generic;

    using UnityEngine;

    using Controllers;
    using Interfaces;
    using Network.Leaderboard;

    using Debug = UnityEngine.Debug;

    public class BasicExamGameEndCommand : INetworkManagerCommand
    {
        private const int LoadLeaderboardTimeoutInSeconds = 10;

        private GameObject endGameUI;

        private GameObject leaderboardUI;

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
            
            this.endGameUI.SetActive(true);

            var endGameUIController = this.endGameUI.GetComponent<EndGameUIController>();
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