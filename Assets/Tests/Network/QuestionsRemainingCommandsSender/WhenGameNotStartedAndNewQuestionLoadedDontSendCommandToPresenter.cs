using DummyEveryBodyVsTheTeacherServer = Tests.DummyObjects.DummyEveryBodyVsTheTeacherServer;
using DummyServerNetworkManager = Tests.DummyObjects.DummyServerNetworkManager;
using ExtendedMonoBehaviour = Utils.Unity.ExtendedMonoBehaviour;

namespace Assets.Tests.Network.QuestionsRemainingCommandsSender
{

    using Assets.Scripts.Commands.UI;
    using Assets.Scripts.Interfaces.Network;
    using Assets.Scripts.Utils;

    using Commands;

    using Interfaces.GameData;
    using Interfaces.Network;
    using Interfaces.Network.NetworkManager;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Zenject;

    public class WhenGameNotStartedAndNewQuestionLoadedDontSendCommandToPresenter : ExtendedMonoBehaviour
    {
        [Inject]
        private IServerNetworkManager networkManager;

        [Inject]
        private IEveryBodyVsTheTeacherServer server;

        [Inject]
        private IGameDataIterator iterator;

        [Inject]
        private IQuestionsRemainingCommandsSender commandsSender;

        void Start()
        {
            var dummyServer = (DummyEveryBodyVsTheTeacherServer)this.server;
            dummyServer.PresenterId = 1;
            dummyServer.IsGameOver = false;
            dummyServer.StartedGame = false;

            var dummyNetworkManager = (DummyServerNetworkManager)this.networkManager;
            dummyNetworkManager.OnSentDataToClient += (sender, args) =>
                {
                    var command = NetworkCommandData.Parse(args.Message);
                    if (command.Name == NetworkManagerCommandUtils.GetCommandName<LoadQuestionRemainingCountCommand>())
                    {
                        IntegrationTest.Fail();
                    }
                };

            this.iterator.GetNextQuestion((question) => {});

            this.CoroutineUtils.WaitForFrames(1, IntegrationTest.Pass);
        }
    }

}