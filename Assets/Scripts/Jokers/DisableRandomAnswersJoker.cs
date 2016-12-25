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
        IGameData gameData;
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

        public bool Activated
        {
            get;
            private set;
        }

        public DisableRandomAnswersJoker()
        {
            this.Image = Resources.Load<Sprite>("Images/Buttons/Jokers/DisableRandomAnswers");
        }

        public DisableRandomAnswersJoker(ClientNetworkManager networkManager, IGameData gameData, IQuestionUIController questionUIController)
        {
            if (networkManager == null)
            {
                throw new ArgumentNullException("networkManager");
            }

            if (gameData == null)
            {
                throw new ArgumentNullException("gameData");
            }
            
            if (questionUIController == null)
            {
                throw new ArgumentNullException("questionUIController");
            }
            
            this.networkManager = networkManager;
            this.questionUIController = questionUIController;
            this.gameData = gameData;

            this.Image = Resources.Load<Sprite>("Images/Buttons/Jokers/DisableRandomAnswers");
        }

        public void Activate()
        {
            if (this.questionUIController.CurrentlyLoadedQuestion == null)
            {
                throw new InvalidOperationException();
            }

            var selectedJokerCommand = new NetworkCommandData("SelectedDisableRandomAnswersJoker");
            this.networkManager.SendServerCommand(selectedJokerCommand);

            var receiveJokerSettings = new ReceivedDisableRandomAnswerJokerSettingsCommand(this.OnReceivedJokerSettings);
            this.networkManager.CommandsManager.AddCommand("DisableRandomAnswersJokerSettings", receiveJokerSettings);

            this.receiveSettingsTimeoutTimer = new Timer(SettingsReceiveTimeoutInSeconds * 1000);
        }

        void OnReceiveSettingsTimeout()
        {
            ThreadUtils.Instance.RunOnMainThread(() =>
                { 
                    this.receiveSettingsTimeoutTimer.Dispose();
                    this.networkManager.CommandsManager.RemoveCommand("DisableRandomAnswersJokerSettings");
                });
        }

        void OnReceivedJokerSettings(int answersToDisableCount)
        {
            this.ActivateJoker(answersToDisableCount);

            this.Activated = true;

            if (this.OnActivated != null)
            {
                this.OnActivated(this, EventArgs.Empty);
            }
        }

        void ActivateJoker(int answersToDisableCount)
        {
            var currentQuestion = this.questionUIController.CurrentlyLoadedQuestion;

            if (answersToDisableCount >= currentQuestion.Answers.Length)
            {
                throw new ArgumentException("Answers to disable count must be less than answers count");
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
        }
    }

}

