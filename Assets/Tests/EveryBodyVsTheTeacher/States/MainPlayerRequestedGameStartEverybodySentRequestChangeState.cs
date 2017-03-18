using EveryBodyVsTheTeacherServer = Network.Servers.EveryBodyVsTheTeacherServer;
using NetworkCommandData = Commands.NetworkCommandData;
using StartGameRequestCommand = Commands.EveryBodyVsTheTeacher.StartGameRequestCommand;

namespace Tests.EveryBodyVsTheTeacher.States
{

    using System;
    using System.Collections;

    using Interfaces.Network.NetworkManager;

    using StateMachine;
    using StateMachine.EveryBodyVsTheTeacher.States.Server;

    using Tests.DummyObjects;
    using Tests.Extensions;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Utils.Unity;

    using Zenject.Source.Usage;

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