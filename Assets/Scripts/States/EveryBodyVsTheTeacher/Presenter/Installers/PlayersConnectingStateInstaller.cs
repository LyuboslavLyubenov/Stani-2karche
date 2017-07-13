using AudiencePlayersContainerUIController = Controllers.EveryBodyVsTheTeacher.PlayersConnecting.AudiencePlayersContainerUIController;
using IClientNetworkManager = Interfaces.Network.NetworkManager.IClientNetworkManager;
using MainPlayersContainerUIController = Controllers.EveryBodyVsTheTeacher.PlayersConnecting.MainPlayersContainerUIController;

namespace Assets.Scripts.States.EveryBodyVsTheTeacher.Presenter
{
    using StateMachine;

    using UnityEngine;

    using Zenject;

    public class PlayersConnectingStateInstaller : MonoInstaller
    {
        [SerializeField]
        private GameObject playersConnectingUI;

        [SerializeField]
        private GameObject mainPlayersContainerUI;

        [SerializeField]
        private GameObject audiencePlayersContainerUI;

        private void OnEveryBodyRequestedGameStart()
        {
            var stateMachine = this.Container.Resolve<StateMachine>();
            var playingState = this.Container.Resolve<PlayingState>();
            stateMachine.SetCurrentState(playingState);
        }

        public override void InstallBindings()
        {
            var clientNetworkManager = this.Container.Resolve<IClientNetworkManager>();
            var mainPlayersContainerUIController = this.mainPlayersContainerUI
                .GetComponent<MainPlayersContainerUIController>();
            var audiencePlayersContainerUIController = this.audiencePlayersContainerUI
                .GetComponent<AudiencePlayersContainerUIController>();
            var playersConnectingState = 
                new PlayersConnectingState(
                    this.playersConnectingUI,
                    mainPlayersContainerUIController,
                    audiencePlayersContainerUIController,
                    clientNetworkManager,
                    this.OnEveryBodyRequestedGameStart);

            this.Container.Bind<PlayersConnectingState>()
                .FromInstance(playersConnectingState)
                .AsSingle();
        }
    }

}