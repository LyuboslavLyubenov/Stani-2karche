using AudiencePlayerConnectedCommand = Commands.EveryBodyVsTheTeacher.PlayersConnectingState.AudiencePlayerConnectedCommand;
using DummyServerNetworkManager = Tests.DummyObjects.DummyServerNetworkManager;
using MainPlayerConnectedCommand = Commands.EveryBodyVsTheTeacher.PlayersConnectingState.MainPlayerConnectedCommand;
using NetworkCommandData = Commands.NetworkCommandData;
using PlayersConnectingToTheServerState = States.EveryBodyVsTheTeacher.Server.PlayersConnectingToTheServerState;

namespace Tests.States.EveryBodyVsTheTeacher.Server.PlayersConnecting
{

    using System.Collections.Generic;
    using System.Linq;

    using Assets.Scripts.Commands.EveryBodyVsTheTeacher;

    using Extensions;

    using Interfaces.Network.NetworkManager;

    using StateMachine;

    using Tests.EveryBodyVsTheTeacher.States.Server.PlayersConnecting;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Utils.Unity;

    using Zenject.Source.Usage;

    public class WhenPresenterConnectedSendConnectedMainPlayersAndAudiencePlayers : ExtendedMonoBehaviour
    {
        private readonly StateMachine stateMachine = new StateMachine();

        [Inject]
        private IServerNetworkManager networkManager;

        [Inject]
        private PlayersConnectingToTheServerState state;

        void Start()
        {
            this.stateMachine.SetCurrentState(this.state);

            this.CoroutineUtils.WaitForFrames(1,
                () =>
                    {
                        var dummyServerNetworkManager = (DummyServerNetworkManager)this.networkManager;
                        dummyServerNetworkManager.SimulateMainPlayerConnected(10, "Ivan");
                        dummyServerNetworkManager.SimulateMainPlayerConnected(20, "Georgi");

                        for (int i = 0; i < 2; i++)
                        {
                            dummyServerNetworkManager.SimulateClientConnected(21 + i, "Audience player" + i);
                        }

                        var actualMainPlayerConnectionIds = new List<int>();
                        var actualAudienceConnectionIds = new List<int>();

                        dummyServerNetworkManager.OnSentDataToClient += (sender, args) =>
                            {
                                var command = NetworkCommandData.Parse(args.Message);
                                if (command.Name == typeof(MainPlayerConnectedCommand).Name.Replace("Command", ""))
                                {
                                    var connectionId = int.Parse(command.Options["ConnectionId"]);
                                    actualMainPlayerConnectionIds.Add(connectionId);
                                }

                                if (command.Name == typeof(AudiencePlayerConnectedCommand).Name.Replace("Command", ""))
                                {
                                    var connectionId = int.Parse(command.Options["ConnectionId"]);
                                    actualAudienceConnectionIds.Add(connectionId);
                                }
                            };
                        
                        this.CoroutineUtils.WaitForFrames(1,
                            () =>
                                {
                                    var presenterConnectingCommand =
                                        NetworkCommandData.From<PresenterConnectingCommand>();
                                    dummyServerNetworkManager.FakeReceiveMessage(1, presenterConnectingCommand.ToString());

                                    if (!this.state.MainPlayersConnectionIds.Except(actualMainPlayerConnectionIds).Any() &&
                                        !this.state.AudiencePlayersConnectionIds.Except(actualAudienceConnectionIds).Any())
                                    {
                                        IntegrationTest.Pass();
                                    }
                                    else
                                    {
                                        IntegrationTest.Fail();
                                    }
                                });
                    });
        }
    }
}