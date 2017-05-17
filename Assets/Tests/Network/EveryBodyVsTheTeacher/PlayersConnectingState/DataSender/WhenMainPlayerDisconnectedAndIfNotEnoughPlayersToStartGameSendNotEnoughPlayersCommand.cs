using NetworkCommandData = Commands.NetworkCommandData;
using NotEnoughPlayersToStartGameCommand = Commands.EveryBodyVsTheTeacher.NotEnoughPlayersToStartGameCommand;

namespace Tests.Network.EveryBodyVsTheTeacher.PlayersConnectingState.DataSender
{

    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    using Interfaces.Network;
    using Interfaces.Network.EveryBodyVsTheTeacher;
    using Interfaces.Network.EveryBodyVsTheTeacher.States;
    using Interfaces.Network.NetworkManager;

    using Tests.DummyObjects;
    using Tests.Extensions;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Utils.Unity;

    using Zenject.Source.Usage;

    public class WhenMainPlayerDisconnectedAndIfNotEnoughPlayersToStartGameSendNotEnoughPlayersCommand : ExtendedMonoBehaviour
    {
        [Inject]
        private IPlayersConnectingToTheServerState state;

        [Inject]
        private IServerNetworkManager networkManager;

        [Inject]
        private IEveryBodyVsTheTeacherServer server;

        [Inject]
        private IPlayersConnectingStateDataSender dataSender;

        private void SimulateMainPlayersConnected(
            DummyServerNetworkManager dummyServerNetworkManager,
            DummyPlayersConnectingToTheServerState dummyState)
        {
            for (int i = 1; i < 7; i++)
            {
                dummyServerNetworkManager.SimulateClientConnected(i, "Ivan");
            }

            dummyState.MainPlayersConnectionIds = new ReadOnlyCollection<int>(Enumerable.Range(1, 7).ToArray());
            dummyState.SimulateMainPlayerConnected(6);
        }

        void Start()
        {
            var dummyServerNetworkManager = (DummyServerNetworkManager)this.networkManager;
            var dummyState = (DummyPlayersConnectingToTheServerState)this.state;
            this.SimulateMainPlayersConnected(dummyServerNetworkManager, dummyState);

            this.CoroutineUtils.WaitForFrames(1,
                () =>
                    {
                        var playersNotReceivedCommand = Enumerable.Range(1, 7).ToList();

                        this.CoroutineUtils.WaitForSeconds(0.5f,
                                () =>
                                    {
                                        if (playersNotReceivedCommand.Count == 0)
                                        {
                                            IntegrationTest.Pass();
                                        }
                                        else
                                        {
                                            IntegrationTest.Fail();
                                        }
                                    });

                        dummyServerNetworkManager.OnSentDataToClient += (sender, args) =>
                            {
                                var command = NetworkCommandData.Parse(args.Message);
                                if (command.Name == typeof(NotEnoughPlayersToStartGameCommand).Name.Replace("Command", ""))
                                {
                                    playersNotReceivedCommand.Remove(args.ConnectionId);
                                }
                            };
                        dummyState.MainPlayersConnectionIds = new ReadOnlyCollection<int>(Enumerable.Range(1, 5).ToArray());
                        dummyState.SimulateMainPlayerDisconnected(6);
                    });
        }
    }
}