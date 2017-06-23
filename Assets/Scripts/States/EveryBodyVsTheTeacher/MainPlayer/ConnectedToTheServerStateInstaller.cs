using IClientNetworkManager = Interfaces.Network.NetworkManager.IClientNetworkManager;

namespace Assets.Scripts.States.EveryBodyVsTheTeacher.MainPlayer
{
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

        public override void InstallBindings()
        {
            this.Container.Bind<ConnectedToServerState>()
                .FromMethod(
                    (context) =>
                        {
                            var clientNetworkManager = context.Container.Resolve<IClientNetworkManager>();
                            return 
                                new ConnectedToServerState(
                                    clientNetworkManager,
                                    this.gameStartButton,
                                    this.questionUI,
                                    this.playingUI,
                                    this.availableJokersUI);
                        })
                .AsSingle();
        }
    }
}