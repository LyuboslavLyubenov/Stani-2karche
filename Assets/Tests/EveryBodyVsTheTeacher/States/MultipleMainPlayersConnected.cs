namespace Tests.EveryBodyVsTheTeacher.States
{

    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using EventArgs;

    using Interfaces.Network.NetworkManager;

    using StateMachine;
    using StateMachine.EveryBodyVsTheTeacher.States.Server;

    using Tests.DummyObjects;
    using Tests.Extensions;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Utils;
    using Utils.Unity;

    using Zenject.Source.Usage;

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

        private void OnMainPlayerConnected(object sender, ClientConnectionIdEventArgs args)
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