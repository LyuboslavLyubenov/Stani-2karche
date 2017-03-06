namespace Assets.Scripts.Interfaces.Network.Leaderboard
{

    using System;

    using Assets.Scripts.EventArgs;

    public interface ILeaderboardReceiver : IDisposable
    {
        event EventHandler<LeaderboardDataEventArgs> OnReceived;
        
        event EventHandler OnError;

        bool Receiving { get; }

        void StartReceiving();
    }
}
