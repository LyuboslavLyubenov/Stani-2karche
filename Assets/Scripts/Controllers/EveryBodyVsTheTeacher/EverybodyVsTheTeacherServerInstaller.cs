namespace Assets.Scripts.Controllers.EveryBodyVsTheTeacher
{

    using Assets.Scripts.Controllers.EveryBodyVsTheTeacher.States.Server;
    using Assets.Scripts.Interfaces.Network.NetworkManager;
    using Assets.Scripts.Network.NetworkManagers;
    using Assets.Zenject.Source.Install;

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