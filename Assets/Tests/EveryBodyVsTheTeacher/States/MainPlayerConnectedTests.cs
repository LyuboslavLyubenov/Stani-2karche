using MainPlayerConnectingCommand = Commands.Server.MainPlayerConnectingCommand;
using NetworkCommandData = Commands.NetworkCommandData;

namespace Tests.EveryBodyVsTheTeacher.States
{

    using System.Collections;

    using EventArgs;

    using Interfaces.Network.NetworkManager;

    using StateMachine;
    using StateMachine.EveryBodyVsTheTeacher.States.Server;

    using Tests.DummyObjects;

    using UnityEngine;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Zenject.Source.Usage;

    public class MainPlayerConnectedTests : MonoBehaviour
    {
        private StateMachine stateMachine = new StateMachine();

        [Inject]
        private PlayersConnectingToTheServerState state;

        [Inject]
        private IServerNetworkManager networkManager;

        void Start()
        {
            this.stateMachine.SetCurrentState(this.state);
            this.state.OnMainPlayerConnected += this.OnMainPlayerConnected;
            
            this.StartCoroutine(this.SimulateClientConnectedToServer());
        }
        
        private IEnumerator SimulateClientConnectedToServer()
        {
            yield return null;

            var dummyServerNetworkManager = ((DummyServerNetworkManager)this.networkManager);

            dummyServerNetworkManager.FakeDisconnectPlayer(1);
            dummyServerNetworkManager.FakeSetUsernameToPlayer(1, "Ivan");

            var mainPlayerConnectingCommand = NetworkCommandData.From<MainPlayerConnectingCommand>();
            dummyServerNetworkManager.FakeReceiveMessage(1, mainPlayerConnectingCommand.ToString());
        }

        private void OnMainPlayerConnected(object sender, ClientConnectionIdEventArgs clientConnectionIdEventArgs)
        {
            if (clientConnectionIdEventArgs.ConnectionId != 1)
            {
                IntegrationTest.Fail("MainPlayer connected but with different connection id");
                return;
            }

            IntegrationTest.Pass();
        }
    }

}