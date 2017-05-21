using EveryBodyVsTheTeacherServer = Network.Servers.EveryBodyVsTheTeacher.EveryBodyVsTheTeacherServer;
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

    using Utils.Unity;

    using Zenject.Source.Usage;

    public class MainPlayerRequestedGameStartNotEverybodySentRequest : ExtendedMonoBehaviour
    {
        private StateMachine stateMachine = new StateMachine();

        [Inject]
        private PlayersConnectingToTheServerState state;

        [Inject]
        private IServerNetworkManager networkManager;

        void Start()
        {
            this.stateMachine.SetCurrentState(this.state);

            this.CoroutineUtils.WaitForFrames(1, this.SimulateMainPlayerConnection);
        }

        private void SimulateMainPlayerConnection()
        {
            var dummyServerNetworkManager = (DummyServerNetworkManager)this.networkManager;

            for (int i = 1; i <= EveryBodyVsTheTeacherServer.MinMainPlayersNeededToStartGame + 1; i++)
            {
                dummyServerNetworkManager.SimulateMainPlayerConnected(i, "Ivan");
            }
            
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