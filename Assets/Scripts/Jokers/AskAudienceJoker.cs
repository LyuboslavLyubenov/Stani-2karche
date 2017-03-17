namespace Assets.Scripts.Jokers
{
    using System;

    using Assets.Scripts.Interfaces.Network.Jokers;
    using Assets.Scripts.Notifications;

    using UnityEngine;

    using Exceptions;
    using Utils.Unity;

    using Controllers;
    using EventArgs;
    using Interfaces;

    using Localization;

    using EventArgs = System.EventArgs;

    public class AskAudienceJoker : IJoker
    {
        public const int MinClientsForOnlineVote_Release = 4;
        public const int MinClientsForOnlineVote_Development = 1;

        public event EventHandler<VoteEventArgs> OnAudienceVoted = delegate
            {
            };

        public event EventHandler OnActivated = delegate
            {
            };

        public event EventHandler<UnhandledExceptionEventArgs> OnError = delegate
            {
            };

        public event EventHandler OnFinishedExecution = delegate
            {
            };

        private GameObject waitingToAnswerUI;
        private GameObject loadingUI;
        private GameObject audienceAnswerUI;

        private IAnswerPollResultRetriever pollDataRetriever;

        public Sprite Image
        {
            get;
            private set;
        }


        public bool Activated
        {
            get;
            private set;
        }

        public AskAudienceJoker(
            IAnswerPollResultRetriever pollDataRetriever,
            GameObject waitingToAnswerUI,
            GameObject audienceAnswerUI,
            GameObject loadingUI)
        {
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

            this.pollDataRetriever.OnReceivedSettingsTimeout += this.OnReceiveSettingsTimeout;
            this.pollDataRetriever.OnReceivedSettings += this.OnReceivedJokerSettings;
            this.pollDataRetriever.OnVoted += this.RetrieverOnVoted;
            this.pollDataRetriever.OnReceivedVoteTimeout += this.OnReceiveVoteTimeout;
        }

        private void OnReceiveSettingsTimeout(object sender, EventArgs args)
        {
            this.loadingUI.SetActive(false);
            this.waitingToAnswerUI.SetActive(false);

            var message = LanguagesManager.Instance.GetValue("Error/NetworkMessages/Timeout");
            NotificationsesController.Instance.AddNotification(Color.red, message);

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

        private void RetrieverOnVoted(object sender, VoteEventArgs args)
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

        private void OnReceiveVoteTimeout(object sender, EventArgs args)
        {
            this.waitingToAnswerUI.SetActive(false);

            var message = LanguagesManager.Instance.GetValue("Error/NetworkMessages/Timeout");
            NotificationsesController.Instance.AddNotification(Color.red, message);

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