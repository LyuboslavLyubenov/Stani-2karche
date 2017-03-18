namespace Network.Leaderboard
{

    using System;
    using System.Collections.Generic;

    using Commands;
    using Commands.Client;

    using DTOs;

    using EventArgs;

    using Interfaces.Network.Leaderboard;

    using Network.NetworkManagers;

    using Utils;

    using EventArgs = System.EventArgs;

    public class LeaderboardReceiver : ILeaderboardReceiver
    {
        public event EventHandler<LeaderboardDataEventArgs> OnReceived = delegate
            {
            };

        public event EventHandler OnError = delegate
            {
            };

        private readonly ClientNetworkManager networkManager;
        private readonly List<PlayerScore> playersScores = new List<PlayerScore>();

        private readonly int timeoutInSeconds = 0;

        public bool Receiving
        {
            get;
            private set;
        }

        public LeaderboardReceiver(ClientNetworkManager networkManager, int timeoutInSeconds)
        {
            if (networkManager == null)
            {
                throw new ArgumentNullException("networkManager");
            }

            if (timeoutInSeconds <= 0)
            {
                throw new ArgumentOutOfRangeException("timeoutInSeconds");
            }

            this.networkManager = networkManager;
            this.timeoutInSeconds = timeoutInSeconds;

            this.Receiving = false;
        }

        private void Timeout()
        {
            this.Receiving = false;
            this.OnError(this, EventArgs.Empty);

            this.networkManager.CommandsManager.RemoveCommand<LeaderboardEntityCommand>();
            this.networkManager.CommandsManager.RemoveCommand("LeaderboardNoMoreEntities");
        }

        private void OnNoMoreEntities(object sender, EventArgs args)
        {
            this.OnReceived(this, new LeaderboardDataEventArgs(this.playersScores));

            this.playersScores.Clear();
            this.Receiving = false;

            this.networkManager.CommandsManager.RemoveCommand<LeaderboardEntityCommand>();
            this.networkManager.CommandsManager.RemoveCommand("LeaderboardNoMoreEntities");
        }

        public void StartReceiving()
        {
            if (this.Receiving)
            {
                throw new InvalidOperationException("Already receiving leaderboard data");
            }

            var receiveLeaderboardEntities = new NetworkCommandData("SendLeaderboardEntities");
            this.networkManager.SendServerCommand(receiveLeaderboardEntities);

            var noMoreEntitiesCommand = new DummyCommand();
            noMoreEntitiesCommand.OnExecuted += this.OnNoMoreEntities;

            this.networkManager.CommandsManager.AddCommand(new LeaderboardEntityCommand(this.playersScores));
            this.networkManager.CommandsManager.AddCommand("LeaderboardNoMoreEntities", noMoreEntitiesCommand);

            this.Receiving = true;

            var timer = TimerUtils.ExecuteAfter(this.timeoutInSeconds,
                () =>
                {
                    if (this.Receiving)
                    {
                        this.Timeout();
                    }
                });

            timer.RunOnUnityThread = true;
            timer.AutoDispose = true;
            timer.Start();
        }

        public void Dispose()
        {
            this.OnReceived = null;
            this.OnError = null;

            try
            {
                this.networkManager.CommandsManager.RemoveCommand<LeaderboardEntityCommand>();
                this.networkManager.CommandsManager.RemoveCommand("LeaderboardNoMoreEntities");
            }
            catch
            {
            }
        }
    }
}