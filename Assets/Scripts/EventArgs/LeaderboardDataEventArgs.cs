using System;
using System.Collections.Generic;

namespace Assets.Scripts.EventArgs
{

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
