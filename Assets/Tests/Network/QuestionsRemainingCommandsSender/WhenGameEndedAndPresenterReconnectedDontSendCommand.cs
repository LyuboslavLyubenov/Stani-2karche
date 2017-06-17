using DummyEveryBodyVsTheTeacherServer = Tests.DummyObjects.DummyEveryBodyVsTheTeacherServer;
using DummyServerNetworkManager = Tests.DummyObjects.DummyServerNetworkManager;
using ExtendedMonoBehaviour = Utils.Unity.ExtendedMonoBehaviour;

namespace Assets.Tests.Network.QuestionsRemainingCommandsSender
{

    using Assets.Scripts.Commands.UI;
    using Assets.Scripts.Utils;
    using Assets.Tests.Extensions;

    using Commands;

    using Interfaces.Network;
    using Interfaces.Network.NetworkManager;
    
    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Zenject.Source.Usage;

    public class WhenGameEndedAndPresenterReconnectedDontSendCommand : ExtendedMonoBehaviour
    {
        [Inject]
        private IServerNetworkManager networkManager;

        [Inject]
        private IEveryBodyVsTheTeacherServer server;

        void Start()
        {
            var dummyServer = (DummyEveryBodyVsTheTeacherServer)this.server;

            dummyServer.PresenterId = 0;
            dummyServer.StartedGame = true;
            dummyServer.IsGameOver = true;

            var dummyNetworkManager = (DummyServerNetworkManager)this.networkManager;
            dummyNetworkManager.OnSentDataToClient += (sender, args) =>
                {
                    var command = NetworkCommandData.Parse(args.Message);
                    if (command.Name == NetworkManagerCommandUtils.GetCommandName<LoadQuestionRemainingCountCommand>())
                    {
                        IntegrationTest.Fail();
                    }
                };

            dummyServer.PresenterId = 1;
            dummyNetworkManager.SimulatePresenterConnected(1);

            this.CoroutineUtils.WaitForFrames(1, IntegrationTest.Pass);
        }
    }
}