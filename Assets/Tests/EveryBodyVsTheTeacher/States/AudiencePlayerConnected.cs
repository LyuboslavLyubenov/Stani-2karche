using FirstRoundState = States.EveryBodyVsTheTeacher.Server.FirstRoundState;

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

    public class AudiencePlayerConnected : ExtendedMonoBehaviour
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