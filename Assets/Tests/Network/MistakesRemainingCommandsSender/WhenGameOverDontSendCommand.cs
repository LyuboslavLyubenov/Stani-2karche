using DummyEveryBodyVsTheTeacherServer = Tests.DummyObjects.DummyEveryBodyVsTheTeacherServer;
using DummyServerNetworkManager = Tests.DummyObjects.DummyServerNetworkManager;
using ExtendedMonoBehaviour = Utils.Unity.ExtendedMonoBehaviour;

namespace Assets.Tests.Network.MistakesRemainingCommandsSender
{

    using Assets.Scripts.Commands.UI;
    using Assets.Scripts.Interfaces.States.EveryBodyVsTheTeacher.Server;
    using Assets.Scripts.Utils;
    using Assets.Tests.DummyObjects;
    using Assets.Tests.Extensions;

    using Commands;

    using Interfaces.Network;
    using Interfaces.Network.NetworkManager;

    using UnityEngine;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Zenject.Source.Usage;

    public class WhenGameOverDontSendCommand : ExtendedMonoBehaviour
    {
        [Inject]
        private IServerNetworkManager networkManager;

        [Inject]
        private IEveryBodyVsTheTeacherServer server;

        [Inject]
        private IRoundsSwitcher roundsSwitcher;

        void Start()
        {
            var dummyServer = (DummyEveryBodyVsTheTeacherServer)this.server;

            dummyServer.IsGameOver = true;
            dummyServer.StartedGame = true;

            var dummyNetworkManager = (DummyServerNetworkManager)this.networkManager;
            dummyNetworkManager.OnSentDataToClient += (sender, args) =>
                {
                    var command = NetworkCommandData.Parse(args.Message);
                    if (command.Name == NetworkManagerCommandUtils.GetCommandName<LoadMistakesRemainingCommand>())
                    {
                        IntegrationTest.Fail();
                    }
                };

            dummyNetworkManager.SimulatePresenterConnected(1);

            var dummyRoundsSwitcher = (DummyRoundsSwitcher)this.roundsSwitcher;
            dummyRoundsSwitcher.FireOnSelectedInCorrectAnswer();
            dummyRoundsSwitcher.SwitchToNextRound();

            this.CoroutineUtils.WaitForFrames(1, IntegrationTest.Pass);
        }
    }
}