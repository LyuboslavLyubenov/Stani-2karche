namespace Assets.Scripts.Jokers
{
    using System;
    using System.Linq;
    using System.Timers;

    using UnityEngine;

    using Assets.Scripts.Exceptions;
    using Assets.Scripts.Network.NetworkManagers;

    using Commands;
    using Commands.Jokers;
    using Interfaces;
    using Network;
    using Utils;

    using EventArgs = System.EventArgs;

    public class DisableRandomAnswersJoker : IJoker
    {
        private const int SettingsReceiveTimeoutInSeconds = 5;

        public event EventHandler OnActivated;
        public event EventHandler<UnhandledExceptionEventArgs> OnError;
        public event EventHandler OnFinishedExecution;

        private ClientNetworkManager networkManager;
        private IQuestionUIController questionUIController;
        private Timer receiveSettingsTimeoutTimer;

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

        public DisableRandomAnswersJoker()
        {
            this.Image = Resources.Load<Sprite>("Images/Buttons/Jokers/DisableRandomAnswers");
        }

        public DisableRandomAnswersJoker(ClientNetworkManager networkManager, IQuestionUIController questionUIController)
        {
            if (networkManager == null)
            {
                throw new ArgumentNullException("networkManager");
            }

            if (questionUIController == null)
            {
                throw new ArgumentNullException("questionUIController");
            }

            this.networkManager = networkManager;
            this.questionUIController = questionUIController;

            this.Image = Resources.Load<Sprite>("Images/Buttons/Jokers/DisableRandomAnswers");
        }
        
        private void OnReceiveSettingsTimeout()
        {
            DisposeTimer();
            this.networkManager.CommandsManager.RemoveCommand<DisableRandomAnswerJokerSettingsCommand>();

            if (OnError != null)
            {
                var exception = new JokerSettingsTimeoutException();
                OnError(this, new UnhandledExceptionEventArgs(exception, true));
            }
        }

        private void OnReceivedJokerSettings(int answersToDisableCount)
        {
            this.ActivateJoker(answersToDisableCount);
        }

        void DisposeTimer()
        {
            this.receiveSettingsTimeoutTimer.Dispose();
            this.receiveSettingsTimeoutTimer = null;
        }

        private void ActivateJoker(int answersToDisableCount)
        {
            var currentQuestion = this.questionUIController.CurrentlyLoadedQuestion;

            if (answersToDisableCount >= currentQuestion.Answers.Length)
            {
                var exception = new ArgumentException("Answers to disable count must be less than answers count");

                if (OnError != null)
                {
                    OnError(this, new UnhandledExceptionEventArgs(exception, true));
                }

                return;
            }

            var allAnswers = currentQuestion.Answers.ToList();
            var correctAnswerIndex = currentQuestion.CorrectAnswerIndex;
            var correctAnswer = allAnswers[correctAnswerIndex];
            var wrongAnswersIndexes = allAnswers.Where(a => a != correctAnswer)
                .ToArray()
                .GetRandomElements(answersToDisableCount)
                .Select(a => allAnswers.FindIndex(answer => answer == a))
                .ToArray();

            for (int i = 0; i < wrongAnswersIndexes.Length; i++)
            {
                var disabledAnswerIndex = wrongAnswersIndexes[i];
                this.questionUIController.HideAnswer(disabledAnswerIndex);
            }

            DisposeTimer();

            if (OnFinishedExecution != null)
            {
                OnFinishedExecution(this, EventArgs.Empty);
            }
        }
        
        public void Activate()
        {
            if (this.questionUIController.CurrentlyLoadedQuestion == null)
            {
                throw new InvalidOperationException();
            }

            var selectedJokerCommand = NetworkCommandData.From<SelectedDisableRandomAnswersJokerCommand>();
            this.networkManager.SendServerCommand(selectedJokerCommand);

            var receiveJokerSettings = new DisableRandomAnswerJokerSettingsCommand(this.OnReceivedJokerSettings);
            this.networkManager.CommandsManager.AddCommand(receiveJokerSettings);

            this.receiveSettingsTimeoutTimer =
                TimerUtils.ExecuteAfter(SettingsReceiveTimeoutInSeconds * 1000, OnReceiveSettingsTimeout);

            if (OnActivated != null)
            {
                OnActivated(this, EventArgs.Empty);
            }

            this.Activated = true;
        }
    }
}