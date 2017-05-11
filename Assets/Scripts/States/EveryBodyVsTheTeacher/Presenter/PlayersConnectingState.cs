using IClientNetworkManager = Interfaces.Network.NetworkManager.IClientNetworkManager;

namespace States.EveryBodyVsTheTeacher.Presenter
{
    using System;

    using Assets.Scripts.Interfaces;
    
    using Interfaces.Controllers;

    using Network.EveryBodyVsTheTeacher.PlayersConnectingState;

    using StateMachine;

    using UnityEngine;

    public class PlayersConnectingState : IState
    {
        private readonly GameObject mainPlayersContainerUI;
        private readonly IMainPlayersContainerUIController mainPlayersContainerUIController;

        private readonly GameObject audiencePlayersContainerUI;
        private readonly IAudiencePlayersContainerUIController audiencePlayersContainerUiController;

        private readonly IClientNetworkManager networkManager;

        private readonly Action onEveryBodyRequestedGameStart;

        public PlayersConnectingState(
            GameObject mainPlayersContainerUI,
            IMainPlayersContainerUIController mainPlayersContainerUIController,
            GameObject audiencePlayersContainerUI,
            IAudiencePlayersContainerUIController audiencePlayersContainerUIController,
            IClientNetworkManager networkManager,
            Action onEveryBodyRequestedGameStart)
        {
            if (mainPlayersContainerUI == null)
            {
                throw new ArgumentNullException("mainPlayersContainerUI");
            }
            
            if (mainPlayersContainerUIController == null)
            {
                throw new ArgumentNullException("mainPlayersContainerUIController");
            }

            if (audiencePlayersContainerUI == null)
            {
                throw new ArgumentNullException("audiencePlayersContainerUI");
            }

            if (audiencePlayersContainerUIController == null)
            {
                throw new ArgumentNullException("audiencePlayersContainerUIController");
            }

            if (networkManager == null)
            {
                throw new ArgumentNullException("networkManager");
            }

            if (onEveryBodyRequestedGameStart == null)
            {
                throw new ArgumentNullException("onEveryBodyRequestedGameStart");
            }

            this.mainPlayersContainerUI = mainPlayersContainerUI;
            this.mainPlayersContainerUIController = mainPlayersContainerUIController;
            this.audiencePlayersContainerUI = audiencePlayersContainerUI;
            this.audiencePlayersContainerUiController = audiencePlayersContainerUIController;
            this.networkManager = networkManager;
            this.onEveryBodyRequestedGameStart = onEveryBodyRequestedGameStart;
        }
        
        public void OnStateEnter(StateMachine stateMachine)
        {
            this.mainPlayersContainerUI.SetActive(true);
            this.audiencePlayersContainerUI.SetActive(true);

            PlayersConnectingStateCommandsInitializer.InitializeCommands(
                this.networkManager,
                this.mainPlayersContainerUIController,
                this.audiencePlayersContainerUiController,
                this.onEveryBodyRequestedGameStart);
        }

        public void OnStateExit(StateMachine stateMachine)
        {
            PlayersConnectingStateCommandsInitializer.CleanCommands(this.networkManager);
        }
    }
}