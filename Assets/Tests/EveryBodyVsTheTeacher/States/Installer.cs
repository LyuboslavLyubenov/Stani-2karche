namespace Assets.Tests.EveryBodyVsTheTeacher.States
{

    using Assets.Scripts.Controllers.EveryBodyVsTheTeacher.States.Server;
    using Assets.Scripts.Interfaces.Network.NetworkManager;
    using Assets.Scripts.StateMachine;
    using Assets.Zenject.Source.Install;

    public class Installer : MonoInstaller
    {
        public override void InstallBindings()
        {
            this.Container.Bind<StateMachine>()
                .FromInstance(new StateMachine());

            this.Container.Bind<PlayersConnectingToTheServerState>()
                .ToSelf();

            this.Container.Bind<GameRunningState>()
                .ToSelf();

            this.Container.Bind<IServerNetworkManager>()
                .To<DummyServerNetworkManager>()
                .AsSingle();
        }
    }

}