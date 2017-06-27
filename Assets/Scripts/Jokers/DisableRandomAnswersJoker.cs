namespace Jokers
{

    using System;
    using System.Linq;

    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Utils;

    using Commands;
    using Commands.Jokers;
    using Commands.Jokers.Selected;

    using Exceptions;

    using Interfaces;
    using Interfaces.Controllers;
    using Interfaces.Network.NetworkManager;

    using UnityEngine;

    using Utils;

    using EventArgs = System.EventArgs;

    public class DisableRandomAnswersJoker : IJoker, IDisposable
    {
        private const int SettingsReceiveTimeoutInSeconds = 5;

        public event EventHandler OnActivated = delegate
            {   
            };

        public event EventHandler<UnhandledExceptionEventArgs> OnError = delegate
            {   
            };

        public event EventHandler OnFinishedExecution = delegate
            {
            };

        private IClientNetworkManager networkManager;
        private IQuestionUIController questionUIController;
        private Timer_ExecuteMethodAfterTime receiveSettingsTimeoutTimer;

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
        
        public DisableRandomAnswersJoker(IClientNetworkManager networkManager, IQuestionUIController questionUIController)
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

            this.Image = JokerUtils.LoadSprite("DisableRandomAnswers");
        }
        
        private void OnReceiveSettingsTimeout()
        {
            this.DisposeTimer();
            this.networkManager.CommandsManager.RemoveCommand<DisableRandomAnswersJokerSettingsCommand>();

            if (this.OnError != null)
            {
                var exception = new JokerSettingsTimeoutException();
                this.OnError(this, new UnhandledExceptionEventArgs(exception, true));
            }
        }

        private void OnReceivedJokerSettings(int answersToDisableCount)
        {
            this.ActivateJoker(answersToDisableCount);
        }

        void DisposeTimer()
        {
            if (this.receiveSettingsTimeoutTimer == null)
            {
                return;
            }

            try
            {
                this.receiveSettingsTimeoutTimer.Stop();
            }
            finally
            {
                this.receiveSettingsTimeoutTimer.Dispose();
                this.receiveSettingsTimeoutTimer = null;
            }
        }

        private void ActivateJoker(int answersToDisableCount)
        {
            var currentQuestion = this.questionUIController.CurrentlyLoadedQuestion;

            if (answersToDisableCount >= currentQuestion.Answers.Length)
            {
                var exception = new ArgumentException("Answers to disable count must be less than answers count");

                if (this.OnError != null)
                {
                    this.OnError(this, new UnhandledExceptionEventArgs(exception, true));
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

            this.Dispose();

            if (this.OnFinishedExecution != null)
            {
                this.OnFinishedExecution(this, EventArgs.Empty);
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

            var receiveJokerSettings = new DisableRandomAnswersJokerSettingsCommand(this.OnReceivedJokerSettings);
            this.networkManager.CommandsManager.AddCommand(receiveJokerSettings);

            this.receiveSettingsTimeoutTimer =
                TimerUtils.ExecuteAfter(SettingsReceiveTimeoutInSeconds, this.OnReceiveSettingsTimeout);

            this.receiveSettingsTimeoutTimer.AutoDispose = true;
            this.receiveSettingsTimeoutTimer.RunOnUnityThread = true;
            this.receiveSettingsTimeoutTimer.Start();

            if (this.OnActivated != null)
            {
                this.OnActivated(this, EventArgs.Empty);
            }

            this.Activated = true;
        }

        public void Dispose()
        {
            this.OnActivated = null;
            this.OnError = null;
            this.OnFinishedExecution = null;
            this.DisposeTimer();
        }
    }
}