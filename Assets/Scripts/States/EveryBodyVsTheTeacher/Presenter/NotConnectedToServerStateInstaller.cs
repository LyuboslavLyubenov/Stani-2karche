using IClientNetworkManager = Interfaces.Network.NetworkManager.IClientNetworkManager;
using NotConnectedToServerState = States.EveryBodyVsTheTeacher.Shared.NotConnectedToServerState;
using UnableToConnectUIController = Controllers.UnableToConnectUIController;

namespace Assets.Scripts.States.EveryBodyVsTheTeacher.Presenter
{

    using UnityEngine;

    using Zenject.Source.Install;

    public class NotConnectedToServerStateInstaller : MonoInstaller
    {
        [SerializeField]
        private GameObject loadingUI;
        
        [SerializeField]
        private GameObject unableToConnectUI;

        public override void InstallBindings()
        {
            var clientNetworkManager = this.Container.Resolve<IClientNetworkManager>();
            var unableToConnectUIController = this.unableToConnectUI.GetComponent<UnableToConnectUIController>();

            var notConnectedToServerState =
                new NotConnectedToServerState(
                    this.loadingUI,
                    this.unableToConnectUI,
                    unableToConnectUIController,
                    clientNetworkManager);

            this.Container.Bind<NotConnectedToServerState>()
                .FromInstance(notConnectedToServerState)
                .AsSingle();
        }
    }

}