namespace Assets.Tests.EveryBodyVsTheTeacher.States
{

    using Assets.Scripts.Controllers.EveryBodyVsTheTeacher.States.Server;
    using Assets.Scripts.EventArgs;
    using Assets.Scripts.Interfaces.Network.NetworkManager;
    using Assets.Scripts.StateMachine;
    using Assets.Scripts.Utils.Unity;
    using Assets.Tests.DummyObjects;
    using Assets.Tests.Extensions;
    using Assets.UnityTestTools.IntegrationTestsFramework.TestRunner;
    using Assets.Zenject.Source.Usage;

    using StateMachine.EveryBodyVsTheTeacher.States.Server;

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
            this.stateMachine.SetCurrentState(state);
            this.state.OnAudiencePlayerConnected += this.OnAudiencePlayerConnected;
            this.CoroutineUtils.WaitForFrames(1, this.SimulateAudiencePlayerConnected);
        }

        private void OnAudiencePlayerConnected(object sender, ClientConnectionDataEventArgs clientConnectionDataEventArgs)
        {
            if (clientConnectionDataEventArgs.ConnectionId == 1)
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