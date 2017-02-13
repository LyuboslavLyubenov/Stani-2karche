namespace Assets.Scripts.Controllers.PlayersConnecting
{

    using Assets.Scripts.Controllers.GameController;
    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Network.NetworkManagers;
    using Assets.Scripts.States.EverybodyVsTheTeacherServer;

    using Zenject;

    public class EverybodyVsTheTeacherServerInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            var serverNetworkManager = ServerNetworkManager.Instance;

            Container.Bind<IServerNetworkManager>()
                .To<ServerNetworkManager>()
                .FromInstance(serverNetworkManager)
                .AsSingle();

            Container.Bind<PlayersConnectingToTheServerState>()
                .AsSingle();
        }
    }

}