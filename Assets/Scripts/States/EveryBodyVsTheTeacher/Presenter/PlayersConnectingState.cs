using IClientNetworkManager = Interfaces.Network.NetworkManager.IClientNetworkManager;

namespace States.EveryBodyVsTheTeacher.Presenter
{
    using System;

    using Assets.Scripts.Interfaces;

    using Controllers.EveryBodyVsTheTeacher.PlayersConnecting;

    using Interfaces.Controllers;

    using Network.EveryBodyVsTheTeacher.PlayersConnectingState;

    using StateMachine;

    public class PlayersConnectingState : IState
    {
        private readonly IMainPlayersContainerUIController mainPlayersContainerUiController;
        private readonly IAudiencePlayersContainerUIController audiencePlayersContainerUiController;
        private readonly IClientNetworkManager networkManager;

        private readonly Action onEveryBodyRequestedGameStart;
        
        public PlayersConnectingState(
            IMainPlayersContainerUIController mainPlayersContainerUiController,
            IAudiencePlayersContainerUIController audiencePlayersContainerUiController,
            IClientNetworkManager networkManager,
            Action onEveryBodyRequestedGameStart)
        {
            if (mainPlayersContainerUiController == null)
            {
                throw new ArgumentNullException("mainPlayersContainerUiController");
            }

            if (audiencePlayersContainerUiController == null)
            {
                throw new ArgumentNullException("audiencePlayersContainerUiController");
            }

            if (networkManager == null)
            {
                throw new ArgumentNullException("networkManager");
            }

            if (onEveryBodyRequestedGameStart == null)
            {
                throw new ArgumentNullException("onEveryBodyRequestedGameStart");
            }

            this.mainPlayersContainerUiController = mainPlayersContainerUiController;
            this.audiencePlayersContainerUiController = audiencePlayersContainerUiController;
            this.networkManager = networkManager;
            this.onEveryBodyRequestedGameStart = onEveryBodyRequestedGameStart;
        }
        
        public void OnStateEnter(StateMachine stateMachine)
        {
            ((MainPlayersContainerUIController)this.mainPlayersContainerUiController).gameObject.SetActive(true);
            ((AudiencePlayersContainerUIController)this.audiencePlayersContainerUiController).gameObject.SetActive(true);

            PlayersConnectingStateCommandsInitializer.InitializeCommands(
                this.networkManager,
                this.mainPlayersContainerUiController,
                this.audiencePlayersContainerUiController,
                this.onEveryBodyRequestedGameStart
                );
        }

        public void OnStateExit(StateMachine stateMachine)
        {
            PlayersConnectingStateCommandsInitializer.CleanCommands(this.networkManager);
        }
    }
}