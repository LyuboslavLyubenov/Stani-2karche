using Election_JokerElectionUIController = Controllers.EveryBodyVsTheTeacher.Jokers.Election.JokerElectionUIController;
using IClientNetworkManager = Interfaces.Network.NetworkManager.IClientNetworkManager;

namespace Assets.Scripts.States.EveryBodyVsTheTeacher.MainPlayer
{
    using Assets.Scripts.Interfaces.Network.EveryBodyVsTheTeacher;
    using Assets.Scripts.Network.EveryBodyVsTheTeacher;

    using UnityEngine;
    using UnityEngine.UI;

    using Zenject.Source.Install;
    
    public class ConnectedToTheServerStateInstaller : MonoInstaller
    {
        [SerializeField]
        private Button gameStartButton;

        [SerializeField]
        private GameObject questionUI;

        [SerializeField]
        private GameObject playingUI;

        [SerializeField]
        private GameObject availableJokersUI;

        [SerializeField]
        private GameObject jokerElectionUI;

        [SerializeField]
        private Election_JokerElectionUIController jokerElectionUIController;

        [SerializeField]
        private GameObject successfullyActivatedJoker;

        [SerializeField]
        private GameObject unsuccessfullyActivatedJoker;

        public override void InstallBindings()
        {
            this.Container.Bind<IJokerElectionCommandsBinder>()
                .FromMethod(
                    (context) =>
                        {
                            var networkManager = context.Container.Resolve<IClientNetworkManager>();
                            var jokerElectionCommandsBinder = 
                                new JokerJokerElectionUiCommandsBinder(
                                    networkManager,
                                    this.jokerElectionUIController,
                                    this.jokerElectionUI,
                                    this.successfullyActivatedJoker,
                                    this.unsuccessfullyActivatedJoker);
                            return jokerElectionCommandsBinder;
                        })
                .AsSingle();
            
            this.Container.Bind<ConnectedToServerState>()
                .FromMethod(
                    (context) =>
                        {
                            var clientNetworkManager = context.Container.Resolve<IClientNetworkManager>();
                            var jokerElectionCommandsBinder = context.Container.Resolve<IJokerElectionCommandsBinder>();
                            return 
                                new ConnectedToServerState(
                                    clientNetworkManager,
                                    this.gameStartButton,
                                    this.questionUI,
                                    this.playingUI,
                                    this.availableJokersUI,
                                    jokerElectionCommandsBinder);
                        })
                .AsSingle();
        }
    }
}