namespace Assets.Tests.EveryBodyVsTheTeacher.States
{
    using Assets.Scripts.Commands;
    using Assets.Scripts.Commands.EveryBodyVsTheTeacher;
    using Assets.Scripts.Controllers.EveryBodyVsTheTeacher.States.Server;
    using Assets.Scripts.Controllers.GameController;
    using Assets.Scripts.Interfaces.Network.NetworkManager;
    using Assets.Scripts.StateMachine;
    using Assets.Scripts.Utils.Unity;
    using Assets.Tests.DummyObjects;
    using Assets.Tests.Extensions;
    using Assets.UnityTestTools.IntegrationTestsFramework.TestRunner;
    using Assets.Zenject.Source.Usage;

    using Network.Servers;

    using StateMachine.EveryBodyVsTheTeacher.States.Server;

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