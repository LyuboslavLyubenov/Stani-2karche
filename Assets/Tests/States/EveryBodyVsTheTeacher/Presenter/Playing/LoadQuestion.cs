using DummyClientNetworkManager = Tests.DummyObjects.DummyClientNetworkManager;

namespace Assets.Tests.States.Presenter.Playing
{

    using Assets.Scripts.States.EveryBodyVsTheTeacher.Presenter;

    using Commands;

    using Interfaces;
    using Interfaces.Controllers;
    using Interfaces.Network.NetworkManager;

    using StateMachine;

    using UnityEngine;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Zenject;

    public class LoadQuestion : MonoBehaviour
    {
        [Inject]
        private IClientNetworkManager networkManager;

        [Inject]
        private ISimpleQuestion question;

        [Inject]
        private IElectionQuestionUIController electionQuestionUiController;

        [Inject]
        private StateMachine stateMachine;

        [Inject]
        private PlayingState state;

        void Start()
        {
            this.stateMachine.SetCurrentState(this.state);

            this.electionQuestionUiController.OnQuestionLoaded += (sender, args) => IntegrationTest.Pass();

            var dummyClientNetworkManager = (DummyClientNetworkManager)this.networkManager;
            var loadQuestionCommand = new NetworkCommandData("LoadQuestion");
            var questionJSON = JsonUtility.ToJson(this.question.Serialize());
            loadQuestionCommand.AddOption("QuestionJSON", questionJSON);
            loadQuestionCommand.AddOption("TimeToAnswer", "10");
            dummyClientNetworkManager.FakeReceiveMessage(loadQuestionCommand.ToString());
        }
    }
}