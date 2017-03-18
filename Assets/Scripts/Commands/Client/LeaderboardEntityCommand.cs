namespace Commands.Client
{

    using System.Collections.Generic;

    using DTOs;

    using Interfaces.Network.NetworkManager;

    using UnityEngine;

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