namespace Tests.States.RoundsSwitcherEventsNotifier
{

    using Assets.Scripts.Interfaces.States.EveryBodyVsTheTeacher.Server;
    using Assets.Scripts.Network.EveryBodyVsTheTeacher;
    using Assets.Scripts.States.EveryBodyVsTheTeacher.Server.Rounds;
    using Assets.Tests.DummyObjects.States.EveryBodyVsTheTeacher;
    using Assets.Tests.Extensions;

    using Interfaces.Network.NetworkManager;

    using StateMachine;

    using Tests.DummyObjects;

    using Zenject;

    public class Installer : MonoInstaller
    {
        private RoundsSwitcher BuildRoundsSwitcher(StateMachine stateMachine, DummyRoundState lastRound)
        {
            var roundsSwitcherBuilder = new RoundsSwitcher.Builder(stateMachine);;
            roundsSwitcherBuilder.AddRound(new DummyRoundState());
            roundsSwitcherBuilder.AddRound(lastRound);
            return roundsSwitcherBuilder.Build();
        }

        public override void InstallBindings()
        {
            var serverNetworkManager = DummyServerNetworkManager.Instance;
            var stateMachine = new StateMachine();
            var lastRound = new DummyRoundState();
            var roundsSwitcher = this.BuildRoundsSwitcher(stateMachine, lastRound);
            var receiverConnectionId = 1;
            
            serverNetworkManager.SimulateClientConnected(1, "Client");

            Container.Bind<IServerNetworkManager>()
                .FromInstance(serverNetworkManager);

            Container.Bind<IRoundsSwitcher>()
                .FromInstance(roundsSwitcher);

            Container.Bind<int>()
                .FromInstance(receiverConnectionId);

            Container.Bind<IRoundState>()
                .FromInstance(lastRound);

            Container.Bind<RoundsSwitcherEventsNotifier>()
                .ToSelf();
        }
    }

}