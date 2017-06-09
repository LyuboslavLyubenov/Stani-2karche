using DummyServerNetworkManager = Tests.DummyObjects.DummyServerNetworkManager;
using EnoughPlayersToStartGameCommand = Commands.EveryBodyVsTheTeacher.EnoughPlayersToStartGameCommand;
using NetworkCommandData = Commands.NetworkCommandData;

namespace Tests.Network.EveryBodyVsTheTeacher.PlayersConnectingState.DataSender
{

    using System.Collections.ObjectModel;
    using System.Linq;

    using Assets.Tests.Extensions;

    using Interfaces.Network;
    using Interfaces.Network.EveryBodyVsTheTeacher;
    using Interfaces.Network.EveryBodyVsTheTeacher.States;
    using Interfaces.Network.NetworkManager;

    using Tests.DummyObjects;

    using UnityEngine;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Zenject.Source.Usage;

    public class WhenEnoughPlayersToStartTheGameSendEnoughPlayersCommand : MonoBehaviour
    {
        [Inject]
        private IServerNetworkManager networkManager;

        [Inject]
        private IPlayersConnectingToTheServerState state;

        [Inject]
        private IEveryBodyVsTheTeacherServer server;

        [Inject]
        private IPlayersConnectingStateDataSender dataSender;
        
        private void SimulateMainPlayersConnected()
        {
            var dummyServerNetworkManager = (DummyServerNetworkManager)this.networkManager;

            for (int i = 1; i < 7; i++)
            {
                dummyServerNetworkManager.SimulateClientConnected(i, "Ivan");
            }

            var dummyState = (DummyPlayersConnectingToTheServerState)this.state;
            dummyState.MainPlayersConnectionIds = new ReadOnlyCollection<int>(Enumerable.Range(1, 7).ToArray());
            dummyState.SimulateMainPlayerConnected(6);
        }

        void Start()
        {
            var dummyServerNetworkManager = (DummyServerNetworkManager)this.networkManager;
            dummyServerNetworkManager.OnSentDataToClient += (sender, args) =>
                {
                    var command = NetworkCommandData.Parse(args.Message);
                    if (command.Name == typeof(EnoughPlayersToStartGameCommand).Name.Replace("Command", ""))
                    {
                        IntegrationTest.Pass();
                    }
                };
            
            this.SimulateMainPlayersConnected();
        }
    }

}