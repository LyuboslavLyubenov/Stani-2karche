namespace Assets.Tests.EveryBodyVsTheTeacher.States
{

    using System;
    using System.Collections;

    using Assets.Scripts.Commands;
    using Assets.Scripts.Commands.EveryBodyVsTheTeacher;
    using Assets.Scripts.Controllers.EveryBodyVsTheTeacher.States.Server;
    using Assets.Scripts.Interfaces.Network.NetworkManager;
    using Assets.Scripts.StateMachine;
    using Assets.Scripts.Utils.Unity;
    using Assets.Tests.DummyObjects;
    using Assets.Tests.Extensions;
    using Assets.UnityTestTools.IntegrationTestsFramework.TestRunner;
    using Assets.Zenject.Source.Usage;

    using Network.Servers;

    using StateMachine.EveryBodyVsTheTeacher.States.Server;

    public class MainPlayerRequestedGameStartEverybodySentRequestChangeState : ExtendedMonoBehaviour
    {
        private StateMachine stateMachine = new StateMachine();

        [Inject]
        private PlayersConnectingToTheServerState state;

        [Inject]
        private FirstRoundState firstRoundState;

        [Inject]
        private IServerNetworkManager networkManager;

        void Start()
        {
            this.stateMachine.SetCurrentState(this.state);

            this.state.OnEveryBodyRequestedGameStart += this.OnEveryBodyRequestedGameStart;

            this.StartCoroutine(this.SimulateMainPlayerConnection());
        }

        private void OnEveryBodyRequestedGameStart(object sender, EventArgs eventArgs)
        {
            IntegrationTest.Pass();
        }

        private IEnumerator SimulateMainPlayerConnection()
        {
            var dummyServerNetworkManager = (DummyServerNetworkManager)this.networkManager;

            for (int i = 1; i <= EveryBodyVsTheTeacherServer.MinMainPlayersNeededToStartGame; i++)
            {
                dummyServerNetworkManager.SimulateMainPlayerConnected(i, "Ivan");
                yield return null;
            }
            
            yield return this.SimulateGameRequestStart();
        }

        private IEnumerator SimulateGameRequestStart()
        {
            var dummyServerNetworkManager = (DummyServerNetworkManager)this.networkManager;
            var gameRequestCommand = NetworkCommandData.From<StartGameRequestCommand>();

            for (int i = 1; i <= EveryBodyVsTheTeacherServer.MinMainPlayersNeededToStartGame; i++)
            {
                dummyServerNetworkManager.FakeReceiveMessage(i, gameRequestCommand.ToString());
                yield return null;
            }
        }
        
    }

}