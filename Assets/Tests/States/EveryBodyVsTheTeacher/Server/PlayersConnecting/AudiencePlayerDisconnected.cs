using PlayersConnectingToTheServerState = States.EveryBodyVsTheTeacher.Server.PlayersConnectingToTheServerState;

namespace Tests.EveryBodyVsTheTeacher.States.Server.PlayersConnecting
{

    using Assets.Tests.Extensions;

    using EventArgs;

    using Interfaces.Network.NetworkManager;

    using StateMachine;

    using Tests.DummyObjects;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Utils.Unity;

    using Zenject;

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

        private void OnAudiencePlayerDisconnected(object sender, ClientConnectionIdEventArgs clientConnectionIdEventArgs)
        {
            if (clientConnectionIdEventArgs.ConnectionId == 1)
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