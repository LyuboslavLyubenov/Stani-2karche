using DummyEveryBodyVsTheTeacherServer = Tests.DummyObjects.DummyEveryBodyVsTheTeacherServer;
using DummyServerNetworkManager = Tests.DummyObjects.DummyServerNetworkManager;
using ExtendedMonoBehaviour = Utils.Unity.ExtendedMonoBehaviour;
using PlayersConnectingToTheServerState = States.EveryBodyVsTheTeacher.Server.PlayersConnectingToTheServerState;

namespace Assets.Tests.States.EveryBodyVsTheTeacher.Server.PlayersConnecting
{
    using System.Linq;

    using Assets.Tests.Extensions;

    using Commands;
    using Commands.EveryBodyVsTheTeacher;

    using Interfaces.Network;
    using Interfaces.Network.NetworkManager;

    using StateMachine;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Zenject;

    public class WhenMainPlayerDisconnectsFromGameAndAllRemainingConnectedPlayersAreReadyForGameStartStartTheGame : ExtendedMonoBehaviour
    {
        [Inject]
        private IServerNetworkManager networkManager;

        [Inject]
        private IEveryBodyVsTheTeacherServer server;

        [Inject]
        private PlayersConnectingToTheServerState state;

        void Start()
        {
            var stateMachine = new StateMachine();
            stateMachine.SetCurrentState(this.state);

            var dummyNetworkManager = (DummyServerNetworkManager)this.networkManager;
            var dummyServer = (DummyEveryBodyVsTheTeacherServer)this.server;
        
            for (int i = 1; i <= 8; i++)
            {
                dummyServer.ConnectedMainPlayersConnectionIds = Enumerable.Range(1, i);
                dummyNetworkManager.SimulateMainPlayerConnected(i, "Main player " + i);
            }

            this.CoroutineUtils.WaitForSeconds(1.1f,
                () =>
                    {
                        var startGameRequestCommand = NetworkCommandData.From<StartGameRequestCommand>();

                        for (int i = 1; i <= 7; i++)
                        {
                            dummyNetworkManager.FakeReceiveMessage(i, startGameRequestCommand.ToString());
                        }

                        this.CoroutineUtils.WaitForFrames(1,
                            () =>
                                {
                                    this.state.OnEveryBodyRequestedGameStart += (sender, args) =>
                                    {
                                        IntegrationTest.Pass();
                                    };

                                    dummyServer.ConnectedMainPlayersConnectionIds = Enumerable.Range(1, 7);
                                    dummyNetworkManager.FakeDisconnectPlayer(8);
                                });
                    });
        }
    }
}