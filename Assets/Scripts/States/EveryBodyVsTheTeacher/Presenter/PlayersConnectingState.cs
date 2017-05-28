using IAudiencePlayersContainerUIController = Interfaces.Controllers.IAudiencePlayersContainerUIController;
using IClientNetworkManager = Interfaces.Network.NetworkManager.IClientNetworkManager;
using IMainPlayersContainerUIController = Interfaces.Controllers.IMainPlayersContainerUIController;
using PlayersConnectingStateCommandsInitializer = Network.EveryBodyVsTheTeacher.PlayersConnectingState.PlayersConnectingStateCommandsInitializer;

namespace Assets.Scripts.States.EveryBodyVsTheTeacher.Presenter
{

    using System;

    using Assets.Scripts.Interfaces;

    using StateMachine;

    using UnityEngine;

    public class PlayersConnectingState : IState
    {
        private readonly GameObject playersConnectingUI;
        private readonly IMainPlayersContainerUIController mainPlayersContainerUIController;    
        private readonly IAudiencePlayersContainerUIController audiencePlayersContainerUiController;
        private readonly IClientNetworkManager networkManager;

        private readonly Action onEveryBodyRequestedGameStart;


        public PlayersConnectingState(
            GameObject playersConnectingUI,
            IMainPlayersContainerUIController mainPlayersContainerUIController,
            IAudiencePlayersContainerUIController audiencePlayersContainerUIController,
            IClientNetworkManager networkManager,
            Action onEveryBodyRequestedGameStart)
        {
            if (playersConnectingUI == null)
            {
                throw new ArgumentNullException("playersConnectingUI");
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

            this.playersConnectingUI = playersConnectingUI;
            this.mainPlayersContainerUIController = mainPlayersContainerUIController;
            this.audiencePlayersContainerUiController = audiencePlayersContainerUIController;
            this.networkManager = networkManager;
            this.onEveryBodyRequestedGameStart = onEveryBodyRequestedGameStart;
        }
        
        public void OnStateEnter(StateMachine stateMachine)
        {
            this.playersConnectingUI.SetActive(true);

            PlayersConnectingStateCommandsInitializer.InitializeCommands(
                this.networkManager,
                this.mainPlayersContainerUIController,
                this.audiencePlayersContainerUiController,
                this.onEveryBodyRequestedGameStart);
        }

        public void OnStateExit(StateMachine stateMachine)
        {
            this.mainPlayersContainerUIController.HideAll();
            this.audiencePlayersContainerUiController.HideAll();

            this.playersConnectingUI.SetActive(false);
            
            PlayersConnectingStateCommandsInitializer.CleanCommands(this.networkManager);
        }
    }
}