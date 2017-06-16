using DummyGameDataIterator = Tests.DummyObjects.DummyGameDataIterator;
using DummyServerNetworkManager = Tests.DummyObjects.DummyServerNetworkManager;

namespace Assets.Tests.Network.QuestionsRemainingCommandsSender
{

    using Assets.Scripts.Commands.UI;
    using Assets.Scripts.Interfaces.Network;
    using Assets.Scripts.Utils;

    using Commands;

    using Interfaces.GameData;
    using Interfaces.Network.NetworkManager;

    using UnityEngine;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Zenject.Source.Usage;

    public class SendQuestionsRemainingCountToPresenterWhenNewQuestionLoaded : MonoBehaviour
    {
        [Inject]
        private IServerNetworkManager networkManager;

        [Inject]
        private IGameDataIterator iterator;

        [Inject]
        private IQuestionsRemainingCommandsSender questionsRemainingCommandsSender;

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

            var dummyGameDataIterator = (DummyGameDataIterator)this.iterator;
            dummyGameDataIterator.GetNextQuestion((question) => { });
        }
    }
}