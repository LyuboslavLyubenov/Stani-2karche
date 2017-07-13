using DummyEveryBodyVsTheTeacherServer = Tests.DummyObjects.DummyEveryBodyVsTheTeacherServer;
using DummyServerNetworkManager = Tests.DummyObjects.DummyServerNetworkManager;

namespace Assets.Tests.Network.MistakesRemainingCommandsSender
{
    using Assets.Scripts.Commands.UI;
    using Assets.Scripts.Interfaces.Network;
    using Assets.Scripts.Utils;
    using Assets.Tests.Extensions;

    using Commands;

    using Interfaces.Network;
    using Interfaces.Network.NetworkManager;

    using UnityEngine;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Zenject;

    public class WhenGameStartedAndGameNotEndedAndPresenterConnectingSendMistakesRemaining : MonoBehaviour
    {
        [Inject]
        private IServerNetworkManager networkManager;

        [Inject]
        private IEveryBodyVsTheTeacherServer server;

        [Inject]
        private int remainingMistakesCount;
        
        [Inject]
        private IMistakesRemainingCommandsSender commandsSender;

        void Start()
        {
            var dummyNetworkManager = (DummyServerNetworkManager)this.networkManager;
            var dummyServer = (DummyEveryBodyVsTheTeacherServer)this.server;

            dummyServer.StartedGame = true;
            dummyServer.IsGameOver = false;

            dummyNetworkManager.OnSentDataToClient += (sender, args) =>
                {
                    var command = NetworkCommandData.Parse(args.Message);
                    if (command.Name == NetworkManagerCommandUtils.GetCommandName<LoadMistakesRemainingCommand>() &&
                        command.Options["Count"] == this.remainingMistakesCount.ToString())
                    {
                        IntegrationTest.Pass();
                    }
                };

            dummyNetworkManager.SimulatePresenterConnected(1);
        }
    }
}