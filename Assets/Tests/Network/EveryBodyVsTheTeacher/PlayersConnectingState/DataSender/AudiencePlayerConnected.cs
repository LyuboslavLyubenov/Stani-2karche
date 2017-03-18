﻿using NetworkCommandData = Commands.NetworkCommandData;

namespace Tests.Network.EveryBodyVsTheTeacher.PlayersConnectingState.DataSender
{

    using Interfaces.Network;
    using Interfaces.Network.EveryBodyVsTheTeacher;
    using Interfaces.Network.EveryBodyVsTheTeacher.States;
    using Interfaces.Network.NetworkManager;

    using Tests.DummyObjects;
    using Tests.Extensions;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Utils.Unity;

    using Zenject.Source.Usage;

    public class AudiencePlayerConnected : ExtendedMonoBehaviour
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
                        command.Name == "AudiencePlayerConnected" &&
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
                    });
        }
    }
}