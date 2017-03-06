namespace Assets.Scripts.Interfaces
{

    using System.Collections.Generic;

    using Assets.Scripts.DTOs;

    public interface ILeaderboardDataManipulator
    {
        IList<PlayerScore> Leaderboard
        {
            get;
        }

        bool Loaded
        {
            get;
        }

        string LevelCategory
        {
            get; set;
        }

        bool AllowDublicates
        {
            get; set;
        }

        void LoadDataAsync();

        void SavePlayerScore(PlayerScore playerScore);
    }
}
