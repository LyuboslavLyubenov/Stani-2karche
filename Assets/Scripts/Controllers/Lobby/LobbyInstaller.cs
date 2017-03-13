namespace Assets.Scripts.Controllers.Lobby
{

    using Assets.Scripts.Controllers.Lobby;
    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Interfaces.Network;
    using Assets.Scripts.Interfaces.Network.Kinvey;
    using Assets.Scripts.Interfaces.Services;
    using Assets.Scripts.Network;
    using Assets.Scripts.Network.Broadcast;
    using Assets.Scripts.Network.TcpSockets;
    using Assets.Zenject.Source.Install;

    using UnityEngine;

    public class LobbyInstaller : MonoInstaller
    {
        public BasicExamServerSelectPlayerTypeUIController BasicExamServerSelectPlayerTypeUIController;
        public SelectPlayerTypeUIController EveryBodyVsTheTeacherSelectPlayerTypeUiController;

        public override void InstallBindings()
        {
            this.InstallSelectPlayerTypeRouterBindings();

            Container.Bind<ISimpleTcpServer>()
                .FromInstance(new SimpleTcpServer(7772))
                .AsSingle();

            Container.Bind<ISimpleTcpClient>()
                .To<SimpleTcpClient>()
                .AsSingle();

            var connectToExternalServerUIController = GameObject.FindObjectOfType<ConnectToExternalServerUIController>();

            Container.Bind<ConnectToExternalServerUIController>()
                .FromInstance(connectToExternalServerUIController)
                .AsSingle();

            var serversAvailableUIController = GameObject.FindObjectOfType<ServersAvailableUIController>();

            Container.Bind<ServersAvailableUIController>()
                .FromInstance(serversAvailableUIController)
                .AsSingle();

            Container.Bind<IKinveyWrapper>()
                .To<KinveyWrapper>()
                .AsSingle();

            Container.Bind<ILANServersDiscoverer>()
                .To<LANServersDiscoverer>()
                .AsSingle();

            Container.Bind<CreatedGameInfoReceiver>()
                .AsSingle();

        }
        
        private void InstallSelectPlayerTypeRouterBindings()
        {
            Container.Bind<BasicExamServerSelectPlayerTypeUIController>()
                .FromInstance(this.BasicExamServerSelectPlayerTypeUIController)
                .AsSingle();

            Container.Bind<SelectPlayerTypeUIController>()
                .FromInstance(this.EveryBodyVsTheTeacherSelectPlayerTypeUiController)
                .AsSingle();

            Container.Bind<SelectPlayerTypeRouter>()
                .AsSingle();
        }
    }

}