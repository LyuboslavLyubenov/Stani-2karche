using System;
using System.Timers;

using UnityEngine;

namespace Assets.Scripts.Jokers.AudienceAnswerPoll
{

    using Assets.Scripts.Commands;
    using Assets.Scripts.Commands.Client;
    using Assets.Scripts.Commands.Jokers;
    using Assets.Scripts.EventArgs;
    using Assets.Scripts.Network;
    using Assets.Scripts.Utils;

    using EventArgs = System.EventArgs;

    public class AudienceAnswerPollResultRetriever : MonoBehaviour
    {
        public const int MinClientsForOnlineVote_Release = 4;
        public const int MinClientsForOnlineVote_Development = 1;

        const int SettingsReceiveTimeoutInSeconds = 10;

        static AudienceAnswerPollResultRetriever instance;

        public static AudienceAnswerPollResultRetriever Instance
        {
            get
            {
                if (instance == null)
                {
                    var obj = new GameObject();
                    obj.name = typeof(AudienceAnswerPollResultRetriever).Name;
                    instance = obj.AddComponent<AudienceAnswerPollResultRetriever>();
                }

                return instance;        
            }
        }

        public EventHandler<AudienceVoteEventArgs> OnAudienceVoted = delegate
            {
            };

        public EventHandler<JokerSettingsEventArgs> OnReceivedSettings = delegate
            {
            };

        public EventHandler OnReceiveSettingsTimeout = delegate
            {
            };
    
        public EventHandler OnReceiveAudienceVoteTimeout = delegate
            {
            };
    
        ClientNetworkManager networkManager;

        Timer timer;

        public EventHandler OnActivated
        {
            get;
            set;
        }

        public bool Activated
        {
            get;
            private set;
        }

        void Awake()
        {
            this.networkManager = GameObject.FindObjectOfType<ClientNetworkManager>();
            this.networkManager.OnDisconnectedEvent += this.OnDisconnected;
        }

        void OnDisconnected(object sender, EventArgs args)
        {
            if (!this.Activated)
            {
                return;
            }

            try
            {
                this.timer.Close();
                this.networkManager.CommandsManager.RemoveCommand<AudiencePollSettingsCommand>();
            }
            catch
            {
            }
        }

        void OnReceivedJokerSettings(int timeToAnswerInSeconds)
        {
            var receivedAskAudienceVoteResultCommand = 
                new AudiencePollResultCommand(
                    (votes) => this.OnAudienceVoted(this, new AudienceVoteEventArgs(votes)));

            this.networkManager.CommandsManager.AddCommand(receivedAskAudienceVoteResultCommand);

            this.timer.Close();

            this.timer = new Timer(SettingsReceiveTimeoutInSeconds * 1000);
            this.timer.AutoReset = false;
            this.timer.Elapsed += this.Timer_OnReceiveAudienceVoteTimeout;

            this.OnReceivedSettings(this, new JokerSettingsEventArgs(timeToAnswerInSeconds));
        }

        void Timer_OnReceiveAudienceVoteTimeout(object sender, ElapsedEventArgs args)
        {
            ThreadUtils.Instance.RunOnMainThread(() =>
                {
                    this.Activated = false;
                    this.networkManager.CommandsManager.RemoveCommand<AudiencePollSettingsCommand>();
                    this.OnReceiveAudienceVoteTimeout(this, EventArgs.Empty);
                });
        }

        void Timer_OnReceiveSettingsTimeout(object sender, ElapsedEventArgs args)
        {
            ThreadUtils.Instance.RunOnMainThread(() =>
                {
                    this.timer.Close();
                    this.Activated = false;
                    this.networkManager.CommandsManager.RemoveCommand<AudiencePollSettingsCommand>();
                    this.OnReceiveSettingsTimeout(this, EventArgs.Empty);
                });
        }

        public void Activate()
        {
            var selected = NetworkCommandData.From<SelectedAudiencePollCommand>();
            this.networkManager.SendServerCommand(selected);

            var receiveSettingsCommand = new AudiencePollSettingsCommand(this.OnReceivedJokerSettings);
            this.networkManager.CommandsManager.AddCommand(receiveSettingsCommand);

            this.timer = new Timer(SettingsReceiveTimeoutInSeconds * 1000);
            this.timer.AutoReset = false;
            this.timer.Elapsed += this.Timer_OnReceiveSettingsTimeout;

            this.Activated = true;

            if (this.OnActivated != null)
            {
                this.OnActivated(this, EventArgs.Empty);
            }
        }
    }

}