namespace Tests.EveryBodyVsTheTeacher.States
{

    using EventArgs;

    using Interfaces.Network.NetworkManager;

    using StateMachine;
    using StateMachine.EveryBodyVsTheTeacher.States.Server;

    using Tests.DummyObjects;
    using Tests.Extensions;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Utils.Unity;

    using Zenject.Source.Usage;

    public class MainPlayerDisconnected : ExtendedMonoBehaviour
    {
        private StateMachine stateMachine = new StateMachine();

        [Inject]
        private PlayersConnectingToTheServerState state;
        
        [Inject]
        private IServerNetworkManager networkManager;

        void Start()
        {
            this.stateMachine.SetCurrentState(this.state);
            this.state.OnMainPlayerDisconnected += this.OnMainPlayerDisconnected;
            this.CoroutineUtils.WaitForFrames(1, this.SimulateMainPlayerConnected);
        }

        private void OnMainPlayerDisconnected(object sender, ClientConnectionDataEventArgs clientConnectionDataEventArgs)
        {
            if (clientConnectionDataEventArgs.ConnectionId == 1)
            {
                IntegrationTest.Pass();
            }
            else
            {
                IntegrationTest.Fail();
            }
        }

        private void SimulateMainPlayerConnected()
        {
            var networkManager = (DummyServerNetworkManager)this.networkManager;
            networkManager.SimulateMainPlayerConnected(1, "Ivan");

            this.CoroutineUtils.WaitForFrames(1, this.SimulateMainPlayerDisconnected);
        }

        private void SimulateMainPlayerDisconnected()
        {
            var networkManager = (DummyServerNetworkManager)this.networkManager;
            networkManager.FakeDisconnectPlayer(1);
        }
    }

}