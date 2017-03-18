namespace Controllers.Lobby
{

    using Interfaces.Network;
    using Interfaces.Network.Kinvey;
    using Interfaces.Services;

    using Network;
    using Network.Broadcast;
    using Network.GameInfo;
    using Network.TcpSockets;

    using UnityEngine;

    using Zenject.Source.Install;

    public class LobbyInstaller : MonoInstaller
    {
        public BasicExamServerSelectPlayerTypeUIController BasicExamServerSelectPlayerTypeUIController;
        public SelectPlayerTypeUIController EveryBodyVsTheTeacherSelectPlayerTypeUiController;

        public override void InstallBindings()
        {
            this.InstallSelectPlayerTypeRouterBindings();

            this.Container.Bind<ISimpleTcpServer>()
                .FromInstance(new SimpleTcpServer(7772))
                .AsSingle();

            this.Container.Bind<ISimpleTcpClient>()
                .To<SimpleTcpClient>()
                .AsSingle();

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

            this.Container.Bind<CreatedGameInfoReceiver>()
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