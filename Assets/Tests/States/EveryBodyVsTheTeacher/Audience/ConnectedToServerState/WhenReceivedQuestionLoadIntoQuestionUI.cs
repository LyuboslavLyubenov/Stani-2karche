using DummyClientNetworkManager = Tests.DummyObjects.DummyClientNetworkManager;

namespace Assets.Tests.States.EveryBodyVsTheTeacher.Audience.ConnectedToServerState
{

    using Assets.Tests.States.Presenter.Playing;

    using Commands;

    using Interfaces;
    using Interfaces.Controllers;
    using Interfaces.Network.NetworkManager;
    using Scripts.Interfaces;
    using UnityEngine;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Zenject;

    public class WhenReceivedQuestionLoadIntoQuestionUI : MonoBehaviour
    {
        [Inject]
        private IClientNetworkManager networkManager;

        [Inject]
        private IQuestionUIController questionUIController;

        [Inject]
        private ISimpleQuestion question;

        [Inject]
        private IState state;

        void Start()
        {
            this.questionUIController.OnQuestionLoaded += (sender, args) => IntegrationTest.Pass();

            var dummyClientNetworkManager = (DummyClientNetworkManager)this.networkManager;
            var loadQuestionCommand = NetworkCommandData.From<LoadQuestion>();
            var questionJSON = JsonUtility.ToJson(this.question.Serialize());
            loadQuestionCommand.AddOption("QuestionJSON", questionJSON);
            loadQuestionCommand.AddOption("TimeToAnswer", "10");
            dummyClientNetworkManager.FakeReceiveMessage(loadQuestionCommand.ToString());
        }
    }
}