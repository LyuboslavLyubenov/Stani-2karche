namespace Controllers.Lobby
{
    using Interfaces.Network.Kinvey;
    using Interfaces.Services;

    using Network;
    using Network.Broadcast;

    using UnityEngine;

    using Utils;

    using Zenject;

    public class LobbyInstaller : MonoInstaller
    {
        public BasicExamServerSelectPlayerTypeUIController BasicExamServerSelectPlayerTypeUIController;
        public SelectPlayerTypeUIController EveryBodyVsTheTeacherSelectPlayerTypeUiController;

        public override void InstallBindings()
        {
            var threadUtils = ThreadUtils.Instance;

            this.InstallSelectPlayerTypeRouterBindings();
            
            var connectToExternalServerUIController = GameObject.FindObjectOfType<ConnectToExternalServerUIController>();

            this.Container.Bind<ConnectToExternalServerUIController>()
                .FromInstance(connectToExternalServerUIController)
                .AsSingle();

            var serversAvailableUIController = GameObject.FindObjectOfType<ServersAvailableUIController>();

            this.Container.Bind<ServersAvailableUIController>()
                .FromInstance(serversAvailableUIController)
                .AsSingle();

            this.Container.Bind<IKinveyWrapper>()
                .To<KinveyWrapper>()
                .AsSingle();

            this.Container.Bind<ILANServersDiscoverer>()
                .To<LANServersDiscoverer>()
                .AsSingle();
        }
        
        private void InstallSelectPlayerTypeRouterBindings()
        {
            this.Container.Bind<BasicExamServerSelectPlayerTypeUIController>()
                .FromInstance(this.BasicExamServerSelectPlayerTypeUIController)
                .AsSingle();

            this.Container.Bind<SelectPlayerTypeUIController>()
                .FromInstance(this.EveryBodyVsTheTeacherSelectPlayerTypeUiController)
                .AsSingle();

            this.Container.Bind<SelectPlayerTypeRouter>()
                .AsSingle();
        }
    }
}