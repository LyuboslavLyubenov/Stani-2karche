using PlayersConnectingToTheServerState = States.EveryBodyVsTheTeacher.Server.PlayersConnectingToTheServerState;

namespace Network.Servers.EveryBodyVsTheTeacher
{

    using Interfaces.Network.NetworkManager;

    using Network.NetworkManagers;

    using Zenject.Source.Install;

    public class EverybodyVsTheTeacherServerInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            var serverNetworkManager = ServerNetworkManager.Instance;

            this.Container.Bind<IServerNetworkManager>()
                .To<ServerNetworkManager>()
                .FromInstance(serverNetworkManager)
                .AsSingle();

            this.Container.Bind<PlayersConnectingToTheServerState>()
                .AsSingle();
        }
    }

}