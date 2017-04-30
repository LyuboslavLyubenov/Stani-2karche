using DummyServerNetworkManager = Tests.DummyObjects.DummyServerNetworkManager;
using NetworkCommandData = Commands.NetworkCommandData;

namespace Tests.States.Server.EndGame
{

    using System.Collections.Generic;
    using System.Linq;

    using Assets.Scripts.States.EveryBodyVsTheTeacher.Server;

    using Commands;

    using Extensions;

    using Interfaces.Network.NetworkManager;

    using StateMachine;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Utils.Unity;

    using Zenject.Source.Usage;

    public class SendEveryPlayerEndGameCommand : ExtendedMonoBehaviour
    {
        [Inject]
        private IServerNetworkManager networkManager;

        [Inject]
        private EndGameState endGameState;

        void Start()
        {
            var playersReceivedEndGameCommand = new List<int>();
            var dummyNetworkManager = (DummyServerNetworkManager)this.networkManager;

            for (int i = 1; i <= 3; i++)
            {
                dummyNetworkManager.SimulateClientConnected(i, "Client " + i);
            }
            
            dummyNetworkManager.OnSentDataToClient += (sender, args) =>
                {
                    var command = NetworkCommandData.Parse(args.Message);
                    if (command.Name == "EndGame")
                    {
                       playersReceivedEndGameCommand.Add(args.ConnectionId);                         
                    }
                };

            var stateMachine = new StateMachine();
            stateMachine.SetCurrentState(this.endGameState);

            this.CoroutineUtils.WaitForSeconds(0.5f,
                () =>
                    {
                        var areAllClientsReceivedEndGameCommand =
                            !this.networkManager.ConnectedClientsConnectionId.Except(playersReceivedEndGameCommand)
                                .Any();

                        if (areAllClientsReceivedEndGameCommand)
                        {
                            IntegrationTest.Pass();
                        }
                        else
                        {
                            IntegrationTest.Fail();
                        }
                    });
        }
    }
}