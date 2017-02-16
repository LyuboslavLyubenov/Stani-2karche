namespace Assets.Scripts.Controllers
{

    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Network;
    using Assets.Scripts.Network.Broadcast;
    using Assets.Scripts.Network.TcpSockets;

    using Zenject;

    public class LobbyInstaller : MonoInstaller
    {
        public BasicExamServerSelectPlayerTypeUIController BasicExamServerSelectPlayerTypeUIController;
        public EveryBodyVsTheTeacher.ServerSelectInfoUIController EveryBodyVsTheTeacherSelectInfoUIController;
        
        public override void InstallBindings()
        {
            Container.Bind<BasicExamServerSelectPlayerTypeUIController>()
                .FromInstance(this.BasicExamServerSelectPlayerTypeUIController)
                .AsSingle();

            Container.Bind<EveryBodyVsTheTeacher.ServerSelectInfoUIController>()
                .FromInstance(this.EveryBodyVsTheTeacherSelectInfoUIController)
                .AsSingle();

            Container.Bind<SelectPlayerTypeRouter>()
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