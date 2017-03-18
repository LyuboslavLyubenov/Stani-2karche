namespace Tests.EveryBodyVsTheTeacher.States
{

    using Interfaces.Network.NetworkManager;

    using StateMachine;
    using StateMachine.EveryBodyVsTheTeacher.States.Server;

    using Tests.DummyObjects;

    using Zenject.Source.Install;

    public class Installer : MonoInstaller
    {
        public override void InstallBindings()
        {
            this.Container.Bind<StateMachine>()
                .FromInstance(new StateMachine());

            this.Container.Bind<PlayersConnectingToTheServerState>()
                .ToSelf();

            this.Container.Bind<FirstRoundState>()
                .ToSelf();

            this.Container.Bind<IServerNetworkManager>()
                .To<DummyServerNetworkManager>()
                .AsSingle();
        }
    }

}