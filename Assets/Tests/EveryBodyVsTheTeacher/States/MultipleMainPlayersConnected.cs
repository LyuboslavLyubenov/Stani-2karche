namespace Assets.Tests.EveryBodyVsTheTeacher.States
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using Assets.Scripts.Controllers.EveryBodyVsTheTeacher.States.Server;
    using Assets.Scripts.EventArgs;
    using Assets.Scripts.Interfaces.Network.NetworkManager;
    using Assets.Scripts.StateMachine;
    using Assets.Scripts.Utils;
    using Assets.Scripts.Utils.Unity;
    using Assets.Tests.Extensions;
    using Assets.UnityTestTools.IntegrationTestsFramework.TestRunner;
    using Assets.Zenject.Source.Usage;
    
    public class MultipleMainPlayersConnected : ExtendedMonoBehaviour
    {
        private StateMachine stateMachine = new StateMachine();

        [Inject]
        private PlayersConnectingToTheServerState state;

        [Inject]
        private IServerNetworkManager networkManager;
        
        private readonly List<int> playersIdsNeededToConnect = new List<int>(Enumerable.Range(1, 5));

        void Start()
        {
            var threadUtils = ThreadUtils.Instance;

            this.stateMachine.SetCurrentState(this.state);
            this.state.OnMainPlayerConnected += this.OnMainPlayerConnected;
            this.StartCoroutine(this.SimulateMainPlayersConnectedToServer());
        }

        private void OnMainPlayerConnected(object sender, ClientConnectionDataEventArgs args)
        {
            this.playersIdsNeededToConnect.Remove(args.ConnectionId);
        }

        private IEnumerator SimulateMainPlayersConnectedToServer()
        {
            var dummyServerNetworkManager = ((DummyServerNetworkManager)this.networkManager);

            for (int i = 1; i <= 5; i++)
            {
                dummyServerNetworkManager.SimulateMainPlayerConnected(i, "Ivan " + i);
                yield return null;
            }

            yield return null;
            
            this.CoroutineUtils.WaitForFrames(1, this.Assert);
        }

        private void Assert()
        {
            if (this.playersIdsNeededToConnect.Count != 0)
            {
                IntegrationTest.Fail();
                return;
            }

            IntegrationTest.Pass();
        }
    }
}