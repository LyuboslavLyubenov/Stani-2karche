namespace Assets.Tests.EveryBodyVsTheTeacher.States
{

    using System;
    using System.Collections.Generic;

    using Assets.Scripts.Controllers.EveryBodyVsTheTeacher.States.Server;
    using Assets.Scripts.Controllers.GameController;
    using Assets.Scripts.EventArgs;
    using Assets.Scripts.Interfaces.Network.NetworkManager;
    using Assets.Scripts.StateMachine;
    using Assets.Scripts.Utils.Unity;
    using Assets.Tests.DummyObjects;
    using Assets.Tests.Extensions;
    using Assets.UnityTestTools.IntegrationTestsFramework.TestRunner;
    using Assets.Zenject.Source.Usage;

    using Network.Servers;

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
            this.stateMachine.SetCurrentState(state);

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