using NetworkCommandData = Commands.NetworkCommandData;
using PlayersConnectingToTheServerState = States.EveryBodyVsTheTeacher.Server.PlayersConnectingToTheServerState;
using StartGameRequestCommand = Commands.EveryBodyVsTheTeacher.StartGameRequestCommand;

namespace Tests.EveryBodyVsTheTeacher.States.Server.PlayersConnecting
{

    using Interfaces.Network.NetworkManager;

    using StateMachine;

    using Tests.DummyObjects;
    using Tests.Extensions;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Utils;
    using Utils.Unity;

    using Zenject.Source.Usage;

    public class MainPlayerRequestGameStartNotEnoughPlayers : ExtendedMonoBehaviour
    {
        private StateMachine stateMachine = new StateMachine();

        [Inject]
        private PlayersConnectingToTheServerState state;

        [Inject]
        private IServerNetworkManager networkManager;

        void Start()
        {
            var threadUtils = ThreadUtils.Instance;

            this.stateMachine.SetCurrentState(this.state);

            this.CoroutineUtils.WaitForFrames(1, this.SimulateMainPlayerConnection);
        }

        private void SimulateMainPlayerConnection()
        {
            var dummyServerNetworkManager = (DummyServerNetworkManager)this.networkManager;
            dummyServerNetworkManager.SimulateMainPlayerConnected(1, "Ivan");

            this.CoroutineUtils.WaitForFrames(1, this.SimulateGameRequestStart);
        }

        private void SimulateGameRequestStart()
        {
            var dummyServerNetworkManager = (DummyServerNetworkManager)this.networkManager;
            var gameRequestCommand = NetworkCommandData.From<StartGameRequestCommand>();
            dummyServerNetworkManager.FakeReceiveMessage(1, gameRequestCommand.ToString());

            this.CoroutineUtils.WaitForFrames(1, this.AssertNotChangedState);
        }

        private void AssertNotChangedState()
        {
            if (this.stateMachine.CurrentState == this.state)
            {
                IntegrationTest.Pass();
            }
            else
            {
                IntegrationTest.Fail();
            }
        }
        
    }

}