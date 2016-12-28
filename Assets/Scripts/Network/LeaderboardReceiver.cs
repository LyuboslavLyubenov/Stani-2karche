using System;
using System.Collections.Generic;

namespace Assets.Scripts.Network
{

    using Assets.Scripts.Commands;
    using Assets.Scripts.Commands.Client;
    using Assets.Scripts.Controllers;
    using Assets.Scripts.DTOs;
    using Assets.Scripts.Utils;
    using Assets.Scripts.Utils.Unity;

    using EventArgs = System.EventArgs;

    public class LeaderboardReceiver : ExtendedMonoBehaviour
    {
        public ClientNetworkManager NetworkManager;
        public LeaderboardUIController Leaderboard;

        List<PlayerScore> playersScores = new List<PlayerScore>();

        public bool Receiving
        {
            get;
            private set;
        }

        int elapsedTimeReceivingInSeconds = 0;
        int timeoutInSeconds = 0;

        Action<PlayerScore[]> onReceived;
        Action onError;

        void Start()
        {
            this.CoroutineUtils.WaitForFrames(0, () => this.InitializeCommand());
            this.CoroutineUtils.RepeatEverySeconds(1, () => this.UpdateElapsedTime());
        }

        void UpdateElapsedTime()
        {
            if (this.Receiving)
            {
                this.elapsedTimeReceivingInSeconds++;

                if (this.timeoutInSeconds >= this.elapsedTimeReceivingInSeconds)
                {
                    this.Timeout();
                }
            }
        }

        void Timeout()
        {
            var timeoutCommand = new NetworkCommandData("LeaderboardReceiveTimeout");
            this.NetworkManager.SendServerCommand(timeoutCommand);
            this.Receiving = false;
        }

        void OnNoMoreEntities(object sender, EventArgs args)
        {
            this.onReceived(this.playersScores.ToArray());
            this.playersScores.Clear();
            this.Receiving = false;
        }

        void InitializeCommand()
        {
            var noMoreEntitiesCommand = new DummyCommand();
            noMoreEntitiesCommand.OnExecuted += this.OnNoMoreEntities;

            this.NetworkManager.CommandsManager.AddCommand("LeaderboardEntity", new ReceivedLeaderboardEntityCommand(this.playersScores));
            this.NetworkManager.CommandsManager.AddCommand("LeaderboardNoMoreEntities", noMoreEntitiesCommand);
        }

        void StartReceiving()
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