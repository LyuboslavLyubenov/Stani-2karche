namespace Assets.Scripts.Network.Leaderboard
{
    using System;
    using System.Collections.Generic;

    using Commands;
    using Commands.Client;
    using Controllers;
    using DTOs;
    using NetworkManagers;
    using Utils.Unity;

    using EventArgs = System.EventArgs;

    public class LeaderboardReceiver : ExtendedMonoBehaviour
    {
        public ClientNetworkManager NetworkManager;
        public LeaderboardUIController Leaderboard;

        public bool Receiving
        {
            get;
            private set;
        }

        private List<PlayerScore> playersScores = new List<PlayerScore>();

        private int elapsedTimeReceivingInSeconds = 0;
        private int timeoutInSeconds = 0;

        private Action<PlayerScore[]> onReceived = null;
        private Action onError = null;

        // ReSharper disable once ArrangeTypeMemberModifiers
        void Start()
        {
            this.CoroutineUtils.WaitForFrames(0, this.InitializeCommand);
            this.CoroutineUtils.RepeatEverySeconds(1f, this.UpdateElapsedTime);
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
            this.NetworkManager.SendServerCommand(timeoutCommand);

            this.NetworkManager.CommandsManager.RemoveCommand<LeaderboardEntityCommand>();

            this.Receiving = false;

            this.onError();
        }

        private void OnNoMoreEntities(object sender, EventArgs args)
        {
            this.playersScores.Clear();
            this.Receiving = false;

            this.onReceived(this.playersScores.ToArray());
        }

        private void InitializeCommand()
        {
            var noMoreEntitiesCommand = new DummyCommand();
            noMoreEntitiesCommand.OnExecuted += this.OnNoMoreEntities;

            this.NetworkManager.CommandsManager.AddCommand("LeaderboardEntity", new LeaderboardEntityCommand(this.playersScores));
            this.NetworkManager.CommandsManager.AddCommand("LeaderboardNoMoreEntities", noMoreEntitiesCommand);
        }

        private void StartReceiving()
        {
            var receiveLeaderboardEntities = new NetworkCommandData("SendLeaderboardEntities");
            this.NetworkManager.SendServerCommand(receiveLeaderboardEntities);

            this.Receiving = true;
            this.elapsedTimeReceivingInSeconds = 0;
        }

        public void Receive(Action<PlayerScore[]> onReceived, Action onError, int timeoutInSeconds)
        {
            if (onReceived == null)
            {
                throw new ArgumentNullException("onReceived");
            }

            if (onError == null)
            {
                throw new ArgumentNullException("onError");
            }

            if (timeoutInSeconds <= 0)
            {
                throw new ArgumentOutOfRangeException("timeoutInSeconds");
            }
            
            if (this.Receiving)
            {
                throw new InvalidOperationException("Already receiving leaderboard data");
            }

            this.onReceived = onReceived;
            this.onError = onError;
            this.timeoutInSeconds = timeoutInSeconds;

            this.StartReceiving();
        }
    }
}