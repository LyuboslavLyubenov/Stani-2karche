using EveryBodyVsTheTeacherServer = Network.Servers.EveryBodyVsTheTeacher.EveryBodyVsTheTeacherServer;
using PlayersConnectingToTheServerState = States.EveryBodyVsTheTeacher.Server.PlayersConnectingToTheServerState;

namespace Tests.EveryBodyVsTheTeacher.States.Server.PlayersConnecting
{

    using System.Collections.Generic;

    using Assets.Tests.Extensions;

    using EventArgs;

    using Interfaces.Network.NetworkManager;

    using StateMachine;

    using Tests.DummyObjects;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Utils.Unity;

    using Zenject;

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

        private void OnAudiencePlayerConnected(object sender, ClientConnectionIdEventArgs clientConnectionIdEventArgs)
        {
            if (this.connectedClientsIds.Contains(clientConnectionIdEventArgs.ConnectionId))
            {
                this.connectedClientsIds.Remove(clientConnectionIdEventArgs.ConnectionId);
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