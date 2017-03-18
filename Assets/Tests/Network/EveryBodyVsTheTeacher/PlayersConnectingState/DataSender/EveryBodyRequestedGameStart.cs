namespace Tests.Network.EveryBodyVsTheTeacher.PlayersConnectingStateDataSender
{

    using System.Collections;
    using System.Linq;

    using Assets.Scripts.Commands;
    using Assets.Scripts.Interfaces.Network;
    using Assets.Scripts.Interfaces.Network.EveryBodyVsTheTeacher;
    using Assets.Scripts.Interfaces.Network.EveryBodyVsTheTeacher.States;
    using Assets.Scripts.Interfaces.Network.NetworkManager;
    using Assets.Tests.DummyObjects;
    using Assets.Tests.Extensions;
    using Assets.UnityTestTools.IntegrationTestsFramework.TestRunner;
    using Assets.Zenject.Source.Usage;

    using UnityEngine;

    public class EveryBodyRequestedGameStart : MonoBehaviour
    {
        [Inject]
        private IServerNetworkManager networkManager;

        [Inject]
        private IEveryBodyVsTheTeacherServer server;

        [Inject]
        private IPlayersConnectingToTheServerState state;
        
        [Inject]
        private IPlayersConnectingStateDataSender stateDataSender;

        void Start()
        {
            var dummyServer = (DummyEveryBodyVsTheTeacherServer)this.server;
            dummyServer.StartedGame = true;
            dummyServer.MainPlayersConnectionIds = Enumerable.Range(1, 5);

            var dummyNetworkManager = (DummyServerNetworkManager)this.networkManager;

            dummyNetworkManager.OnSentDataToClient += (sender, args) =>
                {
                    var command = NetworkCommandData.Parse(args.Message);
                    if (args.ConnectionId == this.server.PresenterId &&
                        command.Name == "EveryBodyRequestedGameStart")
                    {
                        IntegrationTest.Pass();
                    }
                };

            var dummyState = (DummyPlayersConnectingToTheServerState)this.state;

            this.StartCoroutine(AllMainPlayersRequestGameStart(this.server, dummyNetworkManager, dummyState));
        }

        IEnumerator AllMainPlayersRequestGameStart(
            IEveryBodyVsTheTeacherServer server,
            DummyServerNetworkManager dummyNetworkManager,
            DummyPlayersConnectingToTheServerState state)
        {
            yield return this.StartCoroutine(SimulateMainPlayersConnected(server, dummyNetworkManager));
            state.SimulateAllMainPlayersRequestedGameStart();
        }

        IEnumerator SimulateMainPlayersConnected(
            IEveryBodyVsTheTeacherServer server, 
            DummyServerNetworkManager dummyNetworkManager)
        {
            for (int i = 0; i < server.MainPlayersConnectionIds.Count(); i++)
            {
                var connectionId = server.MainPlayersConnectionIds.Skip(i).First();
                var username = "Ivan" + connectionId;
                dummyNetworkManager.SimulateClientConnected(connectionId, username);
                yield return null;
            }
        }
    }
}