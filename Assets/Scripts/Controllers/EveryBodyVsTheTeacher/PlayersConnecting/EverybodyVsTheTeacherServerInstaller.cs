namespace Assets.Scripts.Controllers.EveryBodyVsTheTeacher.PlayersConnecting
{

    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Network.NetworkManagers;
    using Assets.Scripts.States.EverybodyVsTheTeacherServer;

    using Zenject;

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