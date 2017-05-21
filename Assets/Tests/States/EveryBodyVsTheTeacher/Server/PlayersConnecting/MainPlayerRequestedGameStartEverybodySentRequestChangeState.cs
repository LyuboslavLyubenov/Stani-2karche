using EveryBodyVsTheTeacherServer = Network.Servers.EveryBodyVsTheTeacher.EveryBodyVsTheTeacherServer;
using FirstRoundState = Assets.Scripts.States.EveryBodyVsTheTeacher.Server.Rounds.FirstRoundState;
using NetworkCommandData = Commands.NetworkCommandData;
using PlayersConnectingToTheServerState = States.EveryBodyVsTheTeacher.Server.PlayersConnectingToTheServerState;
using StartGameRequestCommand = Commands.EveryBodyVsTheTeacher.StartGameRequestCommand;

namespace Tests.EveryBodyVsTheTeacher.States.Server.PlayersConnecting
{

    using System;
    using System.Collections;

    using Assets.Tests.DummyObjects.States.EveryBodyVsTheTeacher;

    using Interfaces.Network.NetworkManager;

    using StateMachine;

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