using FirstRoundState = Assets.Scripts.States.EveryBodyVsTheTeacher.Server.Rounds.FirstRoundState;
using PlayersConnectingToTheServerState = States.EveryBodyVsTheTeacher.Server.PlayersConnectingToTheServerState;

namespace Tests.EveryBodyVsTheTeacher.States.Server.PlayersConnecting
{

    using Interfaces.Network.NetworkManager;

    using StateMachine;

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