using IClientNetworkManager = Interfaces.Network.NetworkManager.IClientNetworkManager;
using TrustRandomPersonJokerSettingsCommand = Scripts.Commands.Jokers.Settings.TrustRandomPersonJokerSettingsCommand;

namespace Assets.Scripts.Jokers.EveryBodyVsTheTeacher.Presenter
{
    using System;

    using Assets.Scripts.Commands.Jokers.Result;
    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Interfaces.Controllers;

    using UnityEngine;

    public class TrustRandomPersonJoker : IJoker
    {
        public event EventHandler OnActivated = delegate { };
        public event EventHandler<UnhandledExceptionEventArgs> OnError = delegate { };
        public event EventHandler OnFinishedExecution = delegate { };

        private readonly IClientNetworkManager networkManager;
        private readonly GameObject loadingUI;
        private readonly GameObject secondsRemainingUI;
        private readonly ISecondsRemainingUIController secondsRemainingUIController;
        private readonly GameObject notReceivedAnswerUI;
        private readonly GameObject playerAnswerUI;
        private readonly IPlayerAnswerUIController playerAnswerUIController;
        
        public Sprite Image { get; private set; }
        
        public bool Activated { get; private set; }

        public TrustRandomPersonJoker(
            IClientNetworkManager networkManager,
            GameObject loadingUI,
            GameObject secondsRemainingUI,
            ISecondsRemainingUIController secondsRemainingUIController,
            GameObject notReceivedAnswerUI,
            GameObject playerAnswerUI,
            IPlayerAnswerUIController playerAnswerUIController)
        {
            if (networkManager == null)
            {
                throw new ArgumentNullException("networkManager");
            }

            if (loadingUI == null)
            {
                throw new ArgumentNullException("loadingUI");
            }

            if (secondsRemainingUI == null)
            {
                throw new ArgumentNullException("secondsRemainingUI");
            }

            if (secondsRemainingUIController == null)
            {
                throw new ArgumentNullException("secondsRemainingUIController");
            }

            if (notReceivedAnswerUI == null)
            {
                throw new ArgumentNullException("notReceivedAnswerUI");
            }

            if (playerAnswerUI == null)
            {
                throw new ArgumentNullException("playerAnswerUI");
            }

            if (playerAnswerUIController == null)
            {
                throw new ArgumentNullException("playerAnswerUIController");
            }

            this.networkManager = networkManager;
            this.loadingUI = loadingUI;
            this.secondsRemainingUI = secondsRemainingUI;
            this.secondsRemainingUIController = secondsRemainingUIController;
            this.notReceivedAnswerUI = notReceivedAnswerUI;
            this.playerAnswerUI = playerAnswerUI;
            this.playerAnswerUIController = playerAnswerUIController;

            this.Image = Resources.Load<Sprite>("TrustRandomPerson");
        }

        public void Activate()
        {
            var settingsCommand = new TrustRandomPersonJokerSettingsCommand(this.secondsRemainingUIController, this.secondsRemainingUI);
            settingsCommand.OnFinishedExecution += OnReceivedSettings;
            this.networkManager.CommandsManager.AddCommand(settingsCommand);
            
            this.loadingUI.SetActive(true);
            this.Activated = true;
            this.OnActivated(this, EventArgs.Empty);
        }

        private void OnReceivedSettings(object sender, EventArgs args)
        {
            this.loadingUI.SetActive(false);
            var resultCommand = 
                new TrustRandomPersonJokerResultCommand(
                    this.secondsRemainingUI, 
                    this.notReceivedAnswerUI, 
                    this.playerAnswerUI, 
                    this.playerAnswerUIController);
            resultCommand.OnFinishedExecution += OnReceivedResult;
            this.networkManager.CommandsManager.AddCommand(resultCommand);
        }

        private void OnReceivedResult(object sender, EventArgs args)
        {
            this.Activated = false;
            this.OnFinishedExecution(this, EventArgs.Empty);
        }
    }
}