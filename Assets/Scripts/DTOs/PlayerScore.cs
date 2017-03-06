namespace Assets.Scripts.DTOs
{

    using System;

    public class PlayerScore
    {
        public PlayerScore(string playerName, int score, DateTime creationDate)
            : this(playerName, score)
        {
            this.CreationDate = creationDate.ToString();
        }

        public PlayerScore(string playerName, int score)
        {
            if (string.IsNullOrEmpty(playerName))
            {
                throw new ArgumentNullException("playerName");
            }

            if (score < 0)
            {
                throw new ArgumentNullException("score");
            }

            this.PlayerName = playerName;
            this.Score = score;
        }

        public static PlayerScore CreateFrom(PlayerScore_Dto playerScoreDto)
        {
            var username = playerScoreDto.PlayerName;
            var score = playerScoreDto.Score;

            if (string.IsNullOrEmpty(playerScoreDto.CreationDate))
            {
                return new PlayerScore(username, score);
            }

            var date = DateTime.Parse(playerScoreDto.CreationDate);
            return new PlayerScore(username, score, date);
        }

        public string PlayerName
        {
            get;
            private set;
        }

        public int Score
        {
            get;
            private set;
        }

        public string CreationDate
        {
            get;
            private set;
        }
    }

    [Serializable]
    public class PlayerScore_Dto
    {
        public string PlayerName;
        public int Score;
        public string CreationDate;

        public PlayerScore_Dto(PlayerScore playerScore)
        {
            if (playerScore == null)
            {
                throw new ArgumentNullException("playerScore");
            }

            this.PlayerName = playerScore.PlayerName;
            this.Score = playerScore.Score;
            this.CreationDate = playerScore.CreationDate;
        }
    }

}