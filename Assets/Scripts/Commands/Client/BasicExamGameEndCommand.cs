namespace Assets.Scripts.Commands.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Assets.Scripts.EventArgs;
    using Assets.Scripts.Interfaces.Network.Leaderboard;

    using UnityEngine;

    using Controllers;
    using Interfaces;

    using Network.Leaderboard;

    using Debug = UnityEngine.Debug;

    public class BasicExamGameEndCommand : INetworkManagerCommand
    {
        private GameObject endGameUI;
        private GameObject leaderboardUI;

        private readonly ILeaderboardReceiver leaderboardReceiver;
        
        public BasicExamGameEndCommand(GameObject endGameUI, GameObject leaderboardUI, ILeaderboardReceiver leaderboardReceiver)
        {
            if (endGameUI == null)
            {
                throw new ArgumentNullException("endGameUI");
            }

            if (leaderboardUI == null)
            {
                throw new ArgumentNullException("leaderboardUI");
            }

            if (leaderboardReceiver == null)
            {
                throw new ArgumentNullException("leaderboardReceiver");
            }
            
            this.endGameUI = endGameUI;
            this.leaderboardUI = leaderboardUI;
            this.leaderboardReceiver = leaderboardReceiver;

            this.leaderboardReceiver.OnReceived += this.OnReceivedLeaderboardData;
            this.leaderboardReceiver.OnError += OnReceiveLeaderboardDataError;
        }

        private void OnReceivedLeaderboardData(object sender, LeaderboardDataEventArgs args)
        {
            var leaderboardUIController = this.leaderboardUI.GetComponent<LeaderboardUIController>();
            leaderboardUIController.Populate(args.LeaderboardData.ToArray());
        }

        private void OnReceiveLeaderboardDataError(object sender, EventArgs args)
        {
            Debug.LogWarning("Cannot load leaderboard data");
        }

        public void Execute(Dictionary<string, string> commandsOptionsValues)
        {
            var mark = int.Parse(commandsOptionsValues["Mark"]);

            this.endGameUI.SetActive(true);

            var endGameUIController = this.endGameUI.GetComponent<EndGameUIController>();
            endGameUIController.SetMark(mark);

            this.leaderboardReceiver.StartReceiving();
        }
    }

}