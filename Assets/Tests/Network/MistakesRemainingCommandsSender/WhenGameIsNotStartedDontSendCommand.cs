using DummyEveryBodyVsTheTeacherServer = Tests.DummyObjects.DummyEveryBodyVsTheTeacherServer;
using DummyServerNetworkManager = Tests.DummyObjects.DummyServerNetworkManager;
using ExtendedMonoBehaviour = Utils.Unity.ExtendedMonoBehaviour;

namespace Assets.Tests.Network.MistakesRemainingCommandsSender
{
    using Assets.Scripts.Commands.UI;
    using Assets.Scripts.Interfaces.Network;
    using Assets.Scripts.Interfaces.States.EveryBodyVsTheTeacher.Server;
    using Assets.Scripts.Utils;
    using Assets.Tests.DummyObjects;
    using Assets.Tests.Extensions;

    using Commands;

    using Interfaces.Network;
    using Interfaces.Network.NetworkManager;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Zenject;

    public class WhenGameIsNotStartedDontSendCommand : ExtendedMonoBehaviour
    {
        [Inject]
        private IServerNetworkManager networkManager;

        [Inject]
        private IEveryBodyVsTheTeacherServer server;

        [Inject]
        private IRoundsSwitcher roundsSwitcher;

        [Inject]
        private int mistakesRemainingCount;
        
        [Inject]
        private IMistakesRemainingCommandsSender commandsSender;

        void Start()
        {
            var dummyNetworkManager = (DummyServerNetworkManager)this.networkManager;
            dummyNetworkManager.OnSentDataToClient += (sender, args) =>
                {
                    var command = NetworkCommandData.Parse(args.Message);
                    if (command.Name == NetworkManagerCommandUtils.GetCommandName<LoadMistakesRemainingCommand>())
                    {
                        IntegrationTest.Fail();
                    }
                };

            var dummyServer = (DummyEveryBodyVsTheTeacherServer)this.server;
            dummyServer.StartedGame = false;
            dummyServer.IsGameOver = false;
            dummyServer.PresenterId = 1;
            
            dummyNetworkManager.SimulatePresenterConnected(1);

            var dummyRoundsSwitcher = (DummyRoundsSwitcher)this.roundsSwitcher;
            dummyRoundsSwitcher.FireOnSelectedInCorrectAnswer();
            dummyRoundsSwitcher.SwitchToNextRound();

            this.CoroutineUtils.WaitForFrames(1, IntegrationTest.Pass);
        }
    }
}