namespace EventArgs
{

    using System;
    using System.Collections.Generic;

    using DTOs;

    using EventArgs = System.EventArgs;

    public class LeaderboardDataEventArgs : EventArgs
    {
        public LeaderboardDataEventArgs(IList<PlayerScore> leaderboardData)
        {
            if (leaderboardData == null)
            {
                throw new ArgumentNullException("leaderboardData");
            }

            this.LeaderboardData = leaderboardData;
        }

        public IList<PlayerScore> LeaderboardData
        {
            get;
            private set;
        }
    }

}
