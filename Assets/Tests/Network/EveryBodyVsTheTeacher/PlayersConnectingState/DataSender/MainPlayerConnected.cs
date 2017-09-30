using NetworkCommandDataClass = Commands.NetworkCommandData;

namespace Tests.Network.EveryBodyVsTheTeacher.PlayersConnectingState.DataSender
{

    using Assets.Tests.Extensions;

    using Interfaces.Network;
    using Interfaces.Network.EveryBodyVsTheTeacher;
    using Interfaces.Network.EveryBodyVsTheTeacher.States;
    using Interfaces.Network.NetworkManager;

    using Tests.DummyObjects;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Utils.Unity;

    using Zenject;

    public class MainPlayerConnected : ExtendedMonoBehaviour
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

            var connectionId = 1;
            var dummyNetworkManager = (DummyServerNetworkManager)this.networkManager;
            dummyNetworkManager.SimulateClientConnected(connectionId, "Ivan");

            dummyNetworkManager.OnSentDataToClient += (sender, args) =>
                {
                    var command = NetworkCommandDataClass.Parse(args.Message);
                    if (args.ConnectionId == this.server.PresenterId && 
                        command.Name == "MainPlayerConnected" &&
                        command.Options["ConnectionId"] == connectionId.ToString() &&
                        command.Options["Username"] == this.networkManager.GetClientUsername(connectionId))
                    {
                        IntegrationTest.Pass();
                    }
                };

            this.CoroutineUtils.WaitForSeconds(0.5f,
                () =>
                {
                    var dummyState = (DummyPlayersConnectingToTheServerState)this.state;
                    dummyState.SimulateMainPlayerConnected(connectionId);
                });
        }
    }
}
