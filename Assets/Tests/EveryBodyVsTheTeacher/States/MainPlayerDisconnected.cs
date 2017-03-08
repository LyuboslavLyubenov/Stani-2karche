namespace Assets.Tests.EveryBodyVsTheTeacher.States
{

    using System;

    using Assets.Scripts.Controllers.EveryBodyVsTheTeacher.States.Server;
    using Assets.Scripts.EventArgs;
    using Assets.Scripts.Interfaces.Network.NetworkManager;
    using Assets.Scripts.StateMachine;
    using Assets.Scripts.Utils.Unity;
    using Assets.Tests.Extensions;
    using Assets.UnityTestTools.IntegrationTestsFramework.TestRunner;
    using Assets.Zenject.Source.Usage;

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