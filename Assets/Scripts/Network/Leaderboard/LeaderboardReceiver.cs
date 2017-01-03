namespace Assets.Scripts.Network.Leaderboard
{
    using System;
    using System.Collections.Generic;

    using EventArgs;

    using Utils;

    using Commands;
    using Commands.Client;

    using DTOs;

    using NetworkManagers;

    using EventArgs = System.EventArgs;

    public class LeaderboardReceiver
    {
        public event EventHandler<LeaderboardDataEventArgs> OnReceived = delegate
            { };

        public event EventHandler OnError = delegate
            { };

        private readonly ClientNetworkManager networkManager;
        private readonly List<PlayerScore> playersScores = new List<PlayerScore>();

        private int elapsedTimeReceivingInSeconds = 0;
        private readonly int timeoutInSeconds = 0;
        
        private Timer_ExecuteMethodEverySeconds updateElapsedTimeTimer;

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

            var noMoreEntitiesCommand = new DummyCommand();
            noMoreEntitiesCommand.OnExecuted += this.OnNoMoreEntities;

            this.networkManager.CommandsManager.AddCommand(new LeaderboardEntityCommand(this.playersScores));
            this.networkManager.CommandsManager.AddCommand("LeaderboardNoMoreEntities", noMoreEntitiesCommand);

            this.updateElapsedTimeTimer = TimerUtils.ExecuteEvery(1f, this.UpdateElapsedTime);
            this.updateElapsedTimeTimer.RunOnUnityThread = true;
            this.updateElapsedTimeTimer.Start();
        }

        ~LeaderboardReceiver()
        {
            this.networkManager.CommandsManager.RemoveCommand<LeaderboardEntityCommand>();
            this.networkManager.CommandsManager.RemoveCommand("LeaderboardNoMoreEntities");

            this.updateElapsedTimeTimer.Stop();
            this.updateElapsedTimeTimer.Dispose();
            this.updateElapsedTimeTimer = null;
        }
        
        private void UpdateElapsedTime()
        {
            if (!this.Receiving)
            {
                return;
            }

            this.elapsedTimeReceivingInSeconds++;

            if (this.timeoutInSeconds >= this.elapsedTimeReceivingInSeconds)
            {
                this.Timeout();
            }
        }

        private void Timeout()
        {
            var timeoutCommand = new NetworkCommandData("LeaderboardReceiveTimeout");
            this.networkManager.SendServerCommand(timeoutCommand);

            this.networkManager.CommandsManager.RemoveCommand<LeaderboardEntityCommand>();

            this.Receiving = false;

            this.OnError(this, EventArgs.Empty);
        }

        private void OnNoMoreEntities(object sender, EventArgs args)
        {
            this.playersScores.Clear();
            this.Receiving = false;

            this.OnReceived(this, new LeaderboardDataEventArgs(this.playersScores));
        }
        
        private void StartReceiving()
        {
            var receiveLeaderboardEntities = new NetworkCommandData("SendLeaderboardEntities");
            this.networkManager.SendServerCommand(receiveLeaderboardEntities);

            this.Receiving = true;
            this.elapsedTimeReceivingInSeconds = 0;
        }

        public void Receive()
        {
            if (this.Receiving)
            {
                throw new InvalidOperationException("Already receiving leaderboard data");
            }
            
            this.StartReceiving();
        }
    }
}