using NetworkCommandData = Commands.NetworkCommandData;

namespace Tests.Network.EveryBodyVsTheTeacher.PlayersConnectingState.DataSender
{

    using System.Linq;

    using Assets.Tests.Extensions;

    using Interfaces.Network;
    using Interfaces.Network.EveryBodyVsTheTeacher;
    using Interfaces.Network.EveryBodyVsTheTeacher.States;
    using Interfaces.Network.NetworkManager;

    using Tests.DummyObjects;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Utils.Unity;

    using Zenject.Source.Usage;

    public class MainPlayerRequestedGameStart : ExtendedMonoBehaviour
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
            var connectionId = 1;
            var username = "Ivan";
            dummyNetworkManager.SimulateClientConnected(connectionId, username);

            dummyNetworkManager.OnSentDataToClient += (sender, args) =>
                {
                    var command = NetworkCommandData.Parse(args.Message);
                    if (args.ConnectionId == this.server.PresenterId &&
                        command.Name == "MainPlayerRequestedGameStart" &&
                        command.Options["ConnectionId"] == connectionId.ToString())
                    {
                        IntegrationTest.Pass();
                    }
                };

            this.CoroutineUtils.WaitForSeconds(0.5f,
                () =>
                    {
                        var dummyState = (DummyPlayersConnectingToTheServerState)this.state;
                        dummyState.SimulateMainPlayerRequestedGameStart(connectionId);
                    });
        }
    }
}