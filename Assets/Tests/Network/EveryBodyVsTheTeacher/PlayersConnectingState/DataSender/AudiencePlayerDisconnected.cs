namespace Tests.Network.EveryBodyVsTheTeacher.PlayersConnectingStateDataSender
{

    using Assets.Scripts.Commands;
    using Assets.Scripts.Interfaces.Network;
    using Assets.Scripts.Interfaces.Network.EveryBodyVsTheTeacher;
    using Assets.Scripts.Interfaces.Network.EveryBodyVsTheTeacher.States;
    using Assets.Scripts.Interfaces.Network.NetworkManager;
    using Assets.Scripts.Utils.Unity;
    using Assets.Tests.DummyObjects;
    using Assets.Tests.Extensions;
    using Assets.UnityTestTools.IntegrationTestsFramework.TestRunner;
    using Assets.Zenject.Source.Usage;

    using UnityEngine;

    public class AudiencePlayerDisconnected : ExtendedMonoBehaviour
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
            var username = "Ivan";
            var dummyNetworkManager = (DummyServerNetworkManager)this.networkManager;
            
            dummyNetworkManager.SimulateClientConnected(connectionId, username);

            dummyNetworkManager.OnSentDataToClient += (sender, args) =>
                {
                    var command = NetworkCommandData.Parse(args.Message);
                    if (args.ConnectionId == this.server.PresenterId &&
                        command.Name == "AudiencePlayerDisconnected" &&
                        command.Options["ConnectionId"] == connectionId.ToString() &&
                        command.Options["Username"] == username)
                    {
                        IntegrationTest.Pass();
                    }
                };

            this.CoroutineUtils.WaitForSeconds(0.5f,
                () =>
                    {
                        var dummyState = (DummyPlayersConnectingToTheServerState)this.state;
                        dummyState.SimulateAudiencePlayerConnected(connectionId);

                        this.CoroutineUtils.WaitForSeconds(0.5f,
                            () =>
                                {
                                    dummyState.SimulateAudiencePlayerDisconnected(connectionId);               
                                });
                    });

        }
    }
}