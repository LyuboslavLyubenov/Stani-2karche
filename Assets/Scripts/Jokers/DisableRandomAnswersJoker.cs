using Extensions;

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

        private readonly IClientNetworkManager networkManager;
        private readonly IQuestionUIController questionUIController;
        private readonly Timer_ExecuteMethodAfterTime receiveSettingsTimeoutTimer;

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

        public DisableRandomAnswersJoker(
            IClientNetworkManager networkManager, 
            IQuestionUIController questionUIController)
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

            this.receiveSettingsTimeoutTimer = 
                TimerUtils.ExecuteAfter(
                SettingsReceiveTimeoutInSeconds, 
                this.OnReceiveSettingsTimeout);
            
            this.receiveSettingsTimeoutTimer.AutoDispose = true;
            this.receiveSettingsTimeoutTimer.RunOnUnityThread = true;
            this.receiveSettingsTimeoutTimer.Stop();

            this.Image = JokerUtils.LoadSprite("DisableRandomAnswers");
        }

        private void OnReceiveSettingsTimeout()
        {
            if (!this.Activated)
            {
                return;
            }

            this.Activated = false;
            this.networkManager.CommandsManager.RemoveCommand<DisableRandomAnswersJokerSettingsCommand>();

            if (this.OnError != null)
            {
                var exception = new JokerSettingsTimeoutException();
                this.OnError(this, new UnhandledExceptionEventArgs(exception, true));
            }
        }

        private void OnReceivedJokerSettings(string[] answersToDisable)
        {
            this.Activated = false;

            if (this.OnFinishedExecution != null)
            {
                this.OnFinishedExecution(this, EventArgs.Empty);    
            }
        }

        private void DisposeTimer()
        {
            try
            {
                this.receiveSettingsTimeoutTimer.Stop();
            }
            finally
            {
                this.receiveSettingsTimeoutTimer.Dispose();
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

            var receiveJokerSettings = new DisableRandomAnswersJokerSettingsCommand(this.questionUIController);
            receiveJokerSettings.OnFinishedExecution += (sender, args) =>
            {
                this.Dispose();
                this.OnFinishedExecution(sender, args);
            };
            
            this.networkManager.CommandsManager.AddCommand(receiveJokerSettings);

            this.receiveSettingsTimeoutTimer.Reset();

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