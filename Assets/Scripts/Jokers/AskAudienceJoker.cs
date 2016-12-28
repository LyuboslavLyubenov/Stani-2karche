using System;
using UnityEngine;

namespace Assets.Scripts.Jokers
{

    using Assets.Scripts.Exceptions;
    using Assets.Scripts.Utils.Unity;

    using Controllers;
    using EventArgs;
    using Interfaces;
    using AudienceAnswerPoll;
    using Localization;
    using Network;
    using Notifications;
    using Utils;

    using EventArgs = System.EventArgs;

    public class AskAudienceJoker : IJoker
    {
        public const int MinClientsForOnlineVote_Release = 4;
        public const int MinClientsForOnlineVote_Development = 1;

        public EventHandler<AudienceVoteEventArgs> OnAudienceVoted = delegate
            {
            };

        ClientNetworkManager networkManager;

        GameObject waitingToAnswerUI;
        GameObject loadingUI;
        GameObject audienceAnswerUI;

        AudienceAnswerPollResultRetriever pollDataRetriever;

        NotificationsServiceController notificationsServiceController;

        public Sprite Image
        {
            get;
            private set;
        }

        public EventHandler OnActivated
        {
            get;
            set;
        }

        public EventHandler<UnhandledExceptionEventArgs> OnError
        {
            get;
            set;
        }

        public EventHandler OnFinishedExecution
        {
            get;
            set;
        }

        public bool Activated
        {
            get;
            private set;
        }

        public AskAudienceJoker(
            ClientNetworkManager networkManager,
            GameObject waitingToAnswerUI,
            GameObject audienceAnswerUI,
            GameObject loadingUI,
            NotificationsServiceController notificationsServiceController)
        {
            if (networkManager == null)
            {
                throw new ArgumentNullException("networkManager");
            }

            if (waitingToAnswerUI == null)
            {
                throw new ArgumentNullException("waitingToAnswerUI");
            }

            if (audienceAnswerUI == null)
            {
                throw new ArgumentNullException("audienceAnswerUI");
            }

            if (loadingUI == null)
            {
                throw new ArgumentNullException("loadingUI");
            }

            if (notificationsServiceController == null)
            {
                throw new ArgumentNullException("notificationsServiceController");
            }

            this.networkManager = networkManager;
            this.waitingToAnswerUI = waitingToAnswerUI;
            this.audienceAnswerUI = audienceAnswerUI;
            this.loadingUI = loadingUI;
            this.pollDataRetriever = AudienceAnswerPollResultRetriever.Instance;
            this.notificationsServiceController = notificationsServiceController;

            this.Image = Resources.Load<Sprite>("Images/Buttons/Jokers/AskAudience");

            this.pollDataRetriever.OnReceiveSettingsTimeout += this.OnReceiveSettingsTimeout;
            this.pollDataRetriever.OnReceivedSettings += this.OnReceivedJokerSettings;
            this.pollDataRetriever.OnAudienceVoted += this.Retriever_OnAudienceVoted;
            this.pollDataRetriever.OnReceiveAudienceVoteTimeout += this.OnReceiveAudienceVoteTimeout;
        }

        void OnReceiveSettingsTimeout(object sender, EventArgs args)
        {
            this.loadingUI.SetActive(false);
            this.waitingToAnswerUI.SetActive(false);

            var message = LanguagesManager.Instance.GetValue("Error/NetworkMessages/Timeout");
            this.notificationsServiceController.AddNotification(Color.red, message);

            if (OnError != null)
            {
                OnError(this, new UnhandledExceptionEventArgs(new JokerSettingsTimeoutException(), true));
            }
        }

        void OnReceivedJokerSettings(object sender, JokerSettingsEventArgs args)
        {
            this.loadingUI.SetActive(false);
            this.waitingToAnswerUI.SetActive(true);
            this.waitingToAnswerUI.GetComponent<DisableAfterDelay>().DelayInSeconds = args.TimeToAnswerInSeconds;
        }

        void Retriever_OnAudienceVoted(object sender, AudienceVoteEventArgs args)
        {
            this.waitingToAnswerUI.SetActive(false);
            this.audienceAnswerUI.SetActive(true);

            var answersVotes = args.AnswersVotes;
            this.audienceAnswerUI.GetComponent<AudienceAnswerUIController>()
                .SetVoteCount(answersVotes, true);

            if (this.OnAudienceVoted != null)
            {
                this.OnAudienceVoted(this, args);
            }

            if (OnFinishedExecution != null)
            {
                OnFinishedExecution(this, EventArgs.Empty);
            }
        }

        void OnReceiveAudienceVoteTimeout(object sender, EventArgs args)
        {
            this.waitingToAnswerUI.SetActive(false);

            var message = LanguagesManager.Instance.GetValue("Error/NetworkMessages/Timeout");
            this.notificationsServiceController.AddNotification(Color.red, message);

            if (OnError != null)
            {
                OnError(this, new UnhandledExceptionEventArgs(new TimeoutException(), true));
            }
        }

        public void Activate()
        {
            this.loadingUI.SetActive(true);
            this.pollDataRetriever.Activate();

            if (this.OnActivated != null)
            {
                this.OnActivated(this, EventArgs.Empty);
            }

            this.Activated = true;
        }
    }

}