namespace Assets.Scripts.Commands.Client
{
    using System.Collections.Generic;

    using Assets.Scripts.Interfaces.Network.NetworkManager;

    using UnityEngine;

    using DTOs;
    using Interfaces;

    public class LeaderboardEntityCommand : INetworkManagerCommand
    {
        private ICollection<PlayerScore> playersScores;

        public LeaderboardEntityCommand(ICollection<PlayerScore> playersScores)
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
            var playerScoreSer = JsonUtility.FromJson<PlayerScore_Dto>(playerScoreJSON);
            var playerScore = PlayerScore.CreateFrom(playerScoreSer);
            this.playersScores.Add(playerScore);
        }
    }
}