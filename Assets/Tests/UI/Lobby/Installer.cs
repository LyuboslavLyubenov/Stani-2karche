namespace Assets.Tests.UI.Lobby
{

    using Assets.Scripts.Controllers;
    using Assets.Scripts.Controllers.Lobby;
    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Network;
    using Assets.Scripts.Network.Broadcast;
    using Assets.Scripts.Network.TcpSockets;

    using Zenject;

    public class Installer : MonoInstaller
    {
        public BasicExamServerSelectPlayerTypeUIController BasicExamServerSelectPlayerTypeUIController;
        public Scripts.Controllers.EveryBodyVsTheTeacher.SelectPlayerTypeUIController EveryBodyVsTheTeacherSelectPlayerTypeUiController;

        public override void InstallBindings()
        {
            Container.Bind<BasicExamServerSelectPlayerTypeUIController>()
                .FromInstance(this.BasicExamServerSelectPlayerTypeUIController)
                .AsSingle();

            Container.Bind<Scripts.Controllers.EveryBodyVsTheTeacher.SelectPlayerTypeUIController>()
                .FromInstance(this.EveryBodyVsTheTeacherSelectPlayerTypeUiController)
                .AsSingle();

            Container.Bind<Scripts.Controllers.SelectPlayerTypeRouter>()
                .AsSingle();

            Container.Bind<ISimpleTcpServer>()
                .FromInstance(new SimpleTcpServer(7772))
                .AsSingle();

            Container.Bind<ISimpleTcpClient>()
                .To<SimpleTcpClient>()
                .AsSingle();

            Container.Bind<ILANServersDiscoveryService>()
                .To<LANServersDiscoveryService>()
                .AsSingle();

            Container.Bind<CreatedGameInfoReceiverService>()
                .AsSingle();
        }
    }
}