using EveryBodyVsTheTeacherServer = Network.Servers.EveryBodyVsTheTeacherServer;
using NetworkCommandData = Commands.NetworkCommandData;
using StartGameRequestCommand = Commands.EveryBodyVsTheTeacher.StartGameRequestCommand;

namespace Tests.EveryBodyVsTheTeacher.States
{

    using Interfaces.Network.NetworkManager;

    using StateMachine;
    using StateMachine.EveryBodyVsTheTeacher.States.Server;

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

            for (int i = 1; i <= EveryBodyVsTheTeacherServer.MinMainPlayersNeededToStartGame; i++)
            {
                dummyServerNetworkManager.SimulateMainPlayerConnected(1, "Ivan");
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