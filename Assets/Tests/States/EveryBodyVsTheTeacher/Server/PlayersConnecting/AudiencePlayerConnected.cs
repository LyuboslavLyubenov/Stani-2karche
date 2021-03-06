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

    public class AudiencePlayerConnected : ExtendedMonoBehaviour
    {
        private StateMachine stateMachine = new StateMachine();

        [Inject]
        private PlayersConnectingToTheServerState state;
        
        [Inject]
        private IServerNetworkManager networkManager;
        
        void Start()
        {
            this.stateMachine.SetCurrentState(this.state);
            this.state.OnAudiencePlayerConnected += this.OnAudiencePlayerConnected;
            this.CoroutineUtils.WaitForFrames(1, this.SimulateAudiencePlayerConnected);
        }

        private void OnAudiencePlayerConnected(object sender, ClientConnectionIdEventArgs clientConnectionIdEventArgs)
        {
            if (clientConnectionIdEventArgs.ConnectionId == 1)
            {
                IntegrationTest.Pass();
                return;
            }

            IntegrationTest.Fail();
        }

        private void SimulateAudiencePlayerConnected()
        {
            var dummyNetworkManager = (DummyServerNetworkManager)this.networkManager;
            dummyNetworkManager.SimulateClientConnected(1, "Ivan");
        }
    }

}