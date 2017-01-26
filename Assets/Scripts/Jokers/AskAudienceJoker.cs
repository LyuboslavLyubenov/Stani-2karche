namespace Assets.Scripts.Jokers
{
    using System;

    using Assets.Scripts.Notifications;

    using UnityEngine;

    using Exceptions;
    using Network.NetworkManagers;
    using Utils.Unity;

    using Controllers;
    using EventArgs;
    using Interfaces;
    using AudienceAnswerPoll;
    using Localization;

    using EventArgs = System.EventArgs;

    public class AskAudienceJoker : IJoker
    {
        public const int MinClientsForOnlineVote_Release = 4;
        public const int MinClientsForOnlineVote_Development = 1;

        public event EventHandler<AudienceVoteEventArgs> OnAudienceVoted = delegate
            {
            };

        private GameObject waitingToAnswerUI;
        private GameObject loadingUI;
        private GameObject audienceAnswerUI;

        private AudienceAnswerPollResultRetriever pollDataRetriever;

        public Sprite Image
        {
            get;
            private set;
        }

        public event EventHandler OnActivated;
        public event EventHandler<UnhandledExceptionEventArgs> OnError;
        public event EventHandler OnFinishedExecution;

        public bool Activated
        {
            get;
            private set;
        }

        public AskAudienceJoker(
            ClientNetworkManager networkManager,
            AudienceAnswerPollResultRetriever pollDataRetriever,
            GameObject waitingToAnswerUI,
            GameObject audienceAnswerUI,
            GameObject loadingUI)
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

            this.waitingToAnswerUI = waitingToAnswerUI;
            this.audienceAnswerUI = audienceAnswerUI;
            this.loadingUI = loadingUI;
            this.pollDataRetriever = pollDataRetriever;

            this.Image = Resources.Load<Sprite>("Images/Buttons/Jokers/AskAudience");

            this.pollDataRetriever.OnReceiveSettingsTimeout += this.OnReceiveSettingsTimeout;
            this.pollDataRetriever.OnReceivedSettings += this.OnReceivedJokerSettings;
            this.pollDataRetriever.OnAudienceVoted += this.Retriever_OnAudienceVoted;
            this.pollDataRetriever.OnReceiveAudienceVoteTimeout += this.OnReceiveAudienceVoteTimeout;
        }

        private void OnReceiveSettingsTimeout(object sender, EventArgs args)
        {
            this.loadingUI.SetActive(false);
            this.waitingToAnswerUI.SetActive(false);

            var message = LanguagesManager.Instance.GetValue("Error/NetworkMessages/Timeout");
            NotificationsServiceController.Instance.AddNotification(Color.red, message);

            if (OnError != null)
            {
                OnError(this, new UnhandledExceptionEventArgs(new JokerSettingsTimeoutException(), true));
            }
        }

        private void OnReceivedJokerSettings(object sender, JokerSettingsEventArgs args)
        {
            this.loadingUI.SetActive(false);
            this.waitingToAnswerUI.SetActive(true);
            this.waitingToAnswerUI.GetComponent<DisableAfterDelay>().DelayInSeconds = args.TimeToAnswerInSeconds;
        }

        private void Retriever_OnAudienceVoted(object sender, AudienceVoteEventArgs args)
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

        private void OnReceiveAudienceVoteTimeout(object sender, EventArgs args)
        {
            this.waitingToAnswerUI.SetActive(false);

            var message = LanguagesManager.Instance.GetValue("Error/NetworkMessages/Timeout");
            NotificationsServiceController.Instance.AddNotification(Color.red, message);

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