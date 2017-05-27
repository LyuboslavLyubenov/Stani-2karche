namespace Assets.Tests.DummyObjects.Network
{
    using System;
    using System.Collections.Generic;

    using DTOs;

    using EventArgs;

    using Interfaces.Network.Leaderboard;

    public class DummyLeaderboardReceiver : ILeaderboardReceiver
    {
        public event EventHandler<LeaderboardDataEventArgs> OnReceived = delegate { };
        public event EventHandler OnError = delegate { };

        public bool Receiving
        {
            get; private set;
        }

        public void SimulateOnReceived(IList<PlayerScore> leaderboard)
        {
            this.OnReceived(this, new LeaderboardDataEventArgs(leaderboard));
        }

        public void SimulateOnError()
        {
            this.OnError(this, EventArgs.Empty);
        }

        public void StartReceiving()
        {
            this.Receiving = true;
        }
        
        public void Dispose()
        {
        }
    }
}