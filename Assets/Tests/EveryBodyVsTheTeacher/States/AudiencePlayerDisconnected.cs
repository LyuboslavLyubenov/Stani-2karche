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

    public class AudiencePlayerDisconnected : ExtendedMonoBehaviour
    {
        private StateMachine stateMachine = new StateMachine();

        [Inject]
        private PlayersConnectingToTheServerState state;

        [Inject]
        private IServerNetworkManager networkManager;

        void Start()
        {
            this.stateMachine.SetCurrentState(this.state);
            this.state.OnAudiencePlayerDisconnected += this.OnAudiencePlayerDisconnected;
            this.CoroutineUtils.WaitForFrames(1, this.SimulateAudiencePlayerConnected);
        }

        private void OnAudiencePlayerDisconnected(object sender, ClientConnectionDataEventArgs clientConnectionDataEventArgs)
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

        private void SimulateAudiencePlayerConnected()
        {
            var networkManager = (DummyServerNetworkManager)this.networkManager;
            networkManager.SimulateMainPlayerConnected(1, "Ivan");

            this.CoroutineUtils.WaitForFrames(1, this.SimulateAudiencePlayerDisconnected);
        }

        private void SimulateAudiencePlayerDisconnected()
        {
            var networkManager = (DummyServerNetworkManager)this.networkManager;
            networkManager.FakeDisconnectPlayer(1);
        }
    }

}