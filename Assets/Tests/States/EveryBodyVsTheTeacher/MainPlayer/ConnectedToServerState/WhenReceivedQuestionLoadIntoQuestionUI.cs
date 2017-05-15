using DummyClientNetworkManager = Tests.DummyObjects.DummyClientNetworkManager;

namespace Assets.Tests.States.EveryBodyVsTheTeacher.MainPlayer.ConnectedToServerState
{

    using Commands;
    using Commands.Client;

    using Interfaces;
    using Interfaces.Controllers;
    using Interfaces.Network.NetworkManager;

    using UnityEngine;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Zenject.Source.Usage;

    public class WhenReceivedQuestionLoadIntoQuestionUI : MonoBehaviour
    {
        [Inject]
        private IClientNetworkManager networkManager;

        [Inject]
        private IQuestionUIController questionUIController;

        [Inject]
        private ISimpleQuestion question;

        void Start()
        {
            var dummyClientNetworkManager = (DummyClientNetworkManager)this.networkManager;
            var loadQuestionCommand = NetworkCommandData.From<LoadQuestionCommand>();
            var questionJSON = JsonUtility.ToJson(this.question.Serialize());
            loadQuestionCommand.AddOption("QuestionJSON", questionJSON);
            loadQuestionCommand.AddOption("TimeToAnswer", "10");

            dummyClientNetworkManager.FakeReceiveMessage(loadQuestionCommand.ToString());

            this.questionUIController.OnQuestionLoaded += (sender, args) =>
                {
                    IntegrationTest.Pass();
                };
        }
    }
}