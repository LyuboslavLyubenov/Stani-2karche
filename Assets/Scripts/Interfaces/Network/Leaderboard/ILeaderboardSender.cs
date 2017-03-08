namespace Assets.Scripts.Interfaces.Network.Leaderboard
{

    using System;

    using Assets.Scripts.EventArgs;

    public interface ILeaderboardSender
    {
        event EventHandler<LeaderboardDataEventArgs> OnSentLeaderboard;
    }
}
