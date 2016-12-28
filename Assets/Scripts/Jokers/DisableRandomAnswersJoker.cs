using System;
using System.Linq;
using System.Timers;

using UnityEngine;

namespace Assets.Scripts.Jokers
{

    using Assets.Scripts.Commands;
    using Assets.Scripts.Commands.Jokers;
    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Network;
    using Assets.Scripts.Utils;

    using EventArgs = System.EventArgs;

    public class DisableRandomAnswersJoker : IJoker
    {
        const int SettingsReceiveTimeoutInSeconds = 5;

        ClientNetworkManager networkManager;
        IQuestionUIController questionUIController;
        Timer receiveSettingsTimeoutTimer;

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

            this.receiveSettingsTimeoutTimer = new Timer(SettingsReceiveTimeoutInSeconds * 1000);
            this.receiveSettingsTimeoutTimer.Elapsed += this.OnReceiveSettingsTimeout;
            this.receiveSettingsTimeoutTimer.Start();

            if (OnActivated != null)
            {
                OnActivated(this, EventArgs.Empty);
            }

            this.Activated = true;
        }

        void OnReceiveSettingsTimeout(object sender, ElapsedEventArgs args)
        {
            ThreadUtils.Instance.RunOnMainThread(() =>
                {
                    this.receiveSettingsTimeoutTimer.Dispose();
                    this.networkManager.CommandsManager.RemoveCommand<DisableRandomAnswerJokerSettingsCommand>();

                    if (OnError != null)
                    {
                        var exception = new JokerSettingsTimeoutException();
                        OnError(this, new UnhandledExceptionEventArgs(exception, true));
                    }
                });
        }

        void OnReceivedJokerSettings(int answersToDisableCount)
        {
            this.ActivateJoker(answersToDisableCount);
        }

        void ActivateJoker(int answersToDisableCount)
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

            if (OnFinishedExecution != null)
            {
                OnFinishedExecution(this, EventArgs.Empty);
            }
        }
    }
}