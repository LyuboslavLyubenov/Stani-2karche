using DummyServerNetworkManager = Tests.DummyObjects.DummyServerNetworkManager;
using NetworkCommandData = Commands.NetworkCommandData;

namespace Tests.States.Server.EndGame
{

    using Assets.Scripts.States.EveryBodyVsTheTeacher.Server;

    using Extensions;

    using Interfaces.Network.NetworkManager;

    using StateMachine;
    
    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Utils.Unity;

    using Zenject.Source.Usage;

    public class WhenPlayerReconnectedSendEndGameCommand : ExtendedMonoBehaviour
    {
        [Inject]
        private IServerNetworkManager networkManager;

        [Inject]
        private EndGameState endGameState;

        void Start()
        {
            var dummyServerNetworkManager = (DummyServerNetworkManager)this.networkManager;

            for (int i = 1; i <= 3; i++)
            {
                dummyServerNetworkManager.SimulateClientConnected(i, "Client " + i);
            }

            var stateMachine = new StateMachine();
            stateMachine.SetCurrentState(this.endGameState);

            var reconnectedClientConnectionId = 2;

            dummyServerNetworkManager.OnSentDataToClient += (sender, args) =>
                {
                    var command = NetworkCommandData.Parse(args.Message);
                    if (command.Name == "EndGame" && args.ConnectionId == reconnectedClientConnectionId)
                    {
                        IntegrationTest.Pass();
                    }
                };

            dummyServerNetworkManager.FakeDisconnectPlayer(reconnectedClientConnectionId);

            this.CoroutineUtils.WaitForSeconds(0.1f,
                () =>
                    {
                        dummyServerNetworkManager.SimulateClientConnected(reconnectedClientConnectionId, "Client reconnected username");
                    });
        }
    }
}