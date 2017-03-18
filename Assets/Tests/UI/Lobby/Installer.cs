using CreatedGameInfoReceiver = Network.GameInfo.CreatedGameInfoReceiver;
using LANServersDiscoverer = Network.Broadcast.LANServersDiscoverer;
using SimpleTcpClient = Network.TcpSockets.SimpleTcpClient;
using SimpleTcpServer = Network.TcpSockets.SimpleTcpServer;

namespace Tests.UI.Lobby
{

    using Controllers;
    using Controllers.Lobby;

    using Interfaces.Network;
    using Interfaces.Services;

    using Zenject.Source.Install;

    public class Installer : MonoInstaller
    {
        public BasicExamServerSelectPlayerTypeUIController BasicExamServerSelectPlayerTypeUIController;
        public SelectPlayerTypeUIController EveryBodyVsTheTeacherSelectPlayerTypeUiController;

        public override void InstallBindings()
        {
            this.Container.Bind<BasicExamServerSelectPlayerTypeUIController>()
                .FromInstance(this.BasicExamServerSelectPlayerTypeUIController)
                .AsSingle();

            this.Container.Bind<SelectPlayerTypeUIController>()
                .FromInstance(this.EveryBodyVsTheTeacherSelectPlayerTypeUiController)
                .AsSingle();

            this.Container.Bind<Controllers.SelectPlayerTypeRouter>()
                .AsSingle();

            this.Container.Bind<ISimpleTcpServer>()
                .FromInstance(new SimpleTcpServer(7772))
                .AsSingle();

            this.Container.Bind<ISimpleTcpClient>()
                .To<SimpleTcpClient>()
                .AsSingle();

            this.Container.Bind<ILANServersDiscoverer>()
                .To<LANServersDiscoverer>()
                .AsSingle();

            this.Container.Bind<CreatedGameInfoReceiver>()
                .AsSingle();
        }
    }
}