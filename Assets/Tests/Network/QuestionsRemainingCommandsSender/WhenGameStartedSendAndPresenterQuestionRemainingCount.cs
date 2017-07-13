using DummyEveryBodyVsTheTeacherServer = Tests.DummyObjects.DummyEveryBodyVsTheTeacherServer;
using DummyServerNetworkManager = Tests.DummyObjects.DummyServerNetworkManager;
using ExtendedMonoBehaviour = Utils.Unity.ExtendedMonoBehaviour;

namespace Assets.Tests.Network.QuestionsRemainingCommandsSender
{
    using Assets.Scripts.Commands.UI;
    using Assets.Scripts.Interfaces.Network;
    using Assets.Scripts.Utils;
    using Assets.Tests.Extensions;

    using Commands;

    using Interfaces.GameData;
    using Interfaces.Network;
    using Interfaces.Network.NetworkManager;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Zenject;

    public class WhenGameStartedSendAndPresenterQuestionRemainingCount : ExtendedMonoBehaviour
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
            var dummyNetworkManager = (DummyServerNetworkManager)this.networkManager;
            dummyNetworkManager.OnSentDataToClient += (sender, args) =>
                {
                    var command = NetworkCommandData.Parse(args.Message);
                    if (command.Name == NetworkManagerCommandUtils.GetCommandName<LoadQuestionRemainingCountCommand>() &&
                        command.Options["Count"] == this.iterator.RemainingQuestionsToNextMark.ToString())
                    {
                        IntegrationTest.Pass();
                    }
                };

            var dummyServer = (DummyEveryBodyVsTheTeacherServer)this.server;
            dummyServer.IsGameOver = false;
            dummyServer.StartedGame = true;
            dummyServer.PresenterId = 0;
            
            dummyNetworkManager.SimulateClientConnected(1, "Presenter");
            dummyNetworkManager.FakeDisconnectPlayer(1);

            this.CoroutineUtils.WaitForFrames(1,
                () =>
                    {
                        dummyServer.PresenterId = 1;
                        dummyNetworkManager.SimulatePresenterConnected(1);
                    });
        }
    }
}