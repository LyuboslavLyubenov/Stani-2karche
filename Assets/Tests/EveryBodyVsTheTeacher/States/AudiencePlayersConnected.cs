using EveryBodyVsTheTeacherServer = Network.Servers.EveryBodyVsTheTeacherServer;

namespace Tests.EveryBodyVsTheTeacher.States
{

    using System.Collections.Generic;

    using EventArgs;

    using Interfaces.Network.NetworkManager;

    using StateMachine;
    using StateMachine.EveryBodyVsTheTeacher.States.Server;

    using Tests.DummyObjects;
    using Tests.Extensions;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Utils.Unity;

    using Zenject.Source.Usage;

    public class AudiencePlayersConnected : ExtendedMonoBehaviour
    {
        private StateMachine stateMachine = new StateMachine();

        [Inject]
        private PlayersConnectingToTheServerState state;
        
        [Inject]
        private IServerNetworkManager networkManager;

        private HashSet<int> connectedClientsIds = new HashSet<int>();

        void Start()
        {
            this.stateMachine.SetCurrentState(this.state);

            this.state.OnAudiencePlayerConnected += this.OnAudiencePlayerConnected;

            this.CoroutineUtils.WaitForFrames(1, this.SimulateAudiencePlayerConnected);
        }

        private void OnAudiencePlayerConnected(object sender, ClientConnectionDataEventArgs clientConnectionDataEventArgs)
        {
            if (this.connectedClientsIds.Contains(clientConnectionDataEventArgs.ConnectionId))
            {
                this.connectedClientsIds.Remove(clientConnectionDataEventArgs.ConnectionId);
            }

            if (this.connectedClientsIds.Count == 0)
            {
                IntegrationTest.Pass();
            }
        }

        private void SimulateAudiencePlayerConnected()
        {
            var dummyNetworkManager = (DummyServerNetworkManager)this.networkManager;

            for (int i = 1; i <= EveryBodyVsTheTeacherServer.MinMainPlayersNeededToStartGame; i++)
            {
                dummyNetworkManager.SimulateClientConnected(i, "Ivan " + i);
                this.connectedClientsIds.Add(i);
            }
        }
    }

}