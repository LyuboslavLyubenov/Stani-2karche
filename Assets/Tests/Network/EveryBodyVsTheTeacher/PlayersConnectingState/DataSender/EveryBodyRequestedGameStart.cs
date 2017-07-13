using NetworkCommandData = Commands.NetworkCommandData;

namespace Tests.Network.EveryBodyVsTheTeacher.PlayersConnectingState.DataSender
{

    using System.Collections;
    using System.Linq;

    using Assets.Tests.Extensions;

    using Interfaces.Network;
    using Interfaces.Network.EveryBodyVsTheTeacher;
    using Interfaces.Network.EveryBodyVsTheTeacher.States;
    using Interfaces.Network.NetworkManager;

    using Tests.DummyObjects;

    using UnityEngine;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Zenject;

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
            dummyServer.ConnectedMainPlayersConnectionIds = Enumerable.Range(1, 5);

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

            this.StartCoroutine(this.AllMainPlayersRequestGameStart(this.server, dummyNetworkManager, dummyState));
        }

        IEnumerator AllMainPlayersRequestGameStart(
            IEveryBodyVsTheTeacherServer server,
            DummyServerNetworkManager dummyNetworkManager,
            DummyPlayersConnectingToTheServerState state)
        {
            yield return this.StartCoroutine(this.SimulateMainPlayersConnected(server, dummyNetworkManager));
            state.SimulateAllMainPlayersRequestedGameStart();
        }

        IEnumerator SimulateMainPlayersConnected(
            IEveryBodyVsTheTeacherServer server, 
            DummyServerNetworkManager dummyNetworkManager)
        {
            for (int i = 0; i < server.ConnectedMainPlayersConnectionIds.Count(); i++)
            {
                var connectionId = server.ConnectedMainPlayersConnectionIds.Skip(i).First();
                var username = "Ivan" + connectionId;
                dummyNetworkManager.SimulateClientConnected(connectionId, username);
                yield return null;
            }
        }
    }
}