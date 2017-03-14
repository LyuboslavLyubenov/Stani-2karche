namespace Assets.Tests.EveryBodyVsTheTeacher.States
{
    using System.Collections;

    using Assets.Scripts.Commands;
    using Assets.Scripts.Commands.Server;
    using Assets.Scripts.Controllers.EveryBodyVsTheTeacher.States.Server;
    using Assets.Scripts.EventArgs;
    using Assets.Scripts.Interfaces.Network.NetworkManager;
    using Assets.Scripts.StateMachine;
    using Assets.Tests.DummyObjects;
    using Assets.UnityTestTools.IntegrationTestsFramework.TestRunner;
    using Assets.Zenject.Source.Usage;

    using UnityEngine;

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

        private void OnMainPlayerConnected(object sender, ClientConnectionDataEventArgs clientConnectionDataEventArgs)
        {
            if (clientConnectionDataEventArgs.ConnectionId != 1)
            {
                IntegrationTest.Fail("MainPlayer connected but with different connection id");
                return;
            }

            IntegrationTest.Pass();
        }
    }

}