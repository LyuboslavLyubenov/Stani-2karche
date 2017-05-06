using IClientNetworkManager = Interfaces.Network.NetworkManager.IClientNetworkManager;
using KalitkoJokerResultCommand = Commands.Jokers.KalitkoJokerResultCommand;

namespace Assets.Scripts.Jokers.Kalitko
{
    using System;

    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Interfaces.Controllers;

    using UnityEngine;

    public class PresenterKalitkoJoker : IJoker
    {
        public event EventHandler OnActivated = delegate { };
        public event EventHandler<UnhandledExceptionEventArgs> OnError = delegate { };
        public event EventHandler OnFinishedExecution = delegate { };

        private readonly IClientNetworkManager networkManager;
        private readonly IKalitkoJokerUIController kalitkoJokerUIController;
        private readonly GameObject kalitkoJokerUI;
        
        public Sprite Image
        {
            get; private set;
        }

        public bool Activated
        {
            get; private set;
        }

        public PresenterKalitkoJoker(
            IClientNetworkManager networkManager, 
            IKalitkoJokerUIController kalitkoJokerUIController, 
            GameObject kalitkoJokerUI)
        {
            if (networkManager == null)
            {
                throw new ArgumentNullException("networkManager");
            }

            if (kalitkoJokerUIController == null)
            {
                throw new ArgumentNullException("kalitkoJokerUIController");
            }

            if (kalitkoJokerUI == null)
            {
                throw new ArgumentNullException("kalitkoJokerUI");
            }

            this.networkManager = networkManager;
            this.kalitkoJokerUIController = kalitkoJokerUIController;
            this.kalitkoJokerUI = kalitkoJokerUI;

            this.Image = Resources.Load<Sprite>("Images/Buttons/Jokers/KalitkoJoker");
        }

        private void OnReceivedResultCommand(object sender, EventArgs args)
        {
            this.networkManager.CommandsManager.RemoveCommand<KalitkoJokerResultCommand>();
            this.Activated = false;
            this.OnFinishedExecution(this, EventArgs.Empty);
        }

        public void Activate()
        {
            this.kalitkoJokerUI.SetActive(true);
            var kalitkoJokerResultCommand = new KalitkoJokerResultCommand(this.kalitkoJokerUIController, this.kalitkoJokerUI);
            kalitkoJokerResultCommand.OnFinishedExecution += OnReceivedResultCommand;
            this.networkManager.CommandsManager.AddCommand(kalitkoJokerResultCommand);
            this.Activated = true;
            this.OnActivated(this, EventArgs.Empty);
        }

    }
}