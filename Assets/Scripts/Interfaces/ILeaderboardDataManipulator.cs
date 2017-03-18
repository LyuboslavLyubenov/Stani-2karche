namespace Interfaces
{

    using System.Collections.Generic;

    using DTOs;

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
