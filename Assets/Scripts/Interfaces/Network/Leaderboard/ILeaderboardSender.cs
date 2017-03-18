namespace Interfaces.Network.Leaderboard
{

    using System;

    using EventArgs;

    public interface ILeaderboardSender
    {
        event EventHandler<LeaderboardDataEventArgs> OnSentLeaderboard;
    }
}
