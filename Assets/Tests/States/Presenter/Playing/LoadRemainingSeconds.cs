using DummyClientNetworkManager = Tests.DummyObjects.DummyClientNetworkManager;

namespace Assets.Tests.States.Presenter.Playing
{
    using Assets.Scripts.Interfaces.Controllers;
    using Assets.Scripts.States.EveryBodyVsTheTeacher.Presenter;

    using Commands;

    using Interfaces;
    using Interfaces.Network.NetworkManager;

    using StateMachine;

    using UnityEngine;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Zenject.Source.Usage;

    public class LoadRemainingSeconds : MonoBehaviour
    {
        [Inject]
        private IClientNetworkManager networkManager;

        [Inject]
        private ISecondsRemainingUIController secondsRemainingUiController;

        [Inject]
        private ISimpleQuestion question;

        [Inject]
        private StateMachine stateMachine;

        [Inject]
        private PlayingState state;

        void Start()
        {
            this.stateMachine.SetCurrentState(this.state);

            var dummyServerNetworkManager = (DummyClientNetworkManager)this.networkManager;
            var questionJSON = JsonUtility.ToJson(this.question.Serialize());
            var timeToAnswerInSeconds = 10;
            var command = new NetworkCommandData("LoadQuestion");

            command.AddOption("QuestionJSON", questionJSON);
            command.AddOption("TimeToAnswer", timeToAnswerInSeconds.ToString());

            dummyServerNetworkManager.FakeReceiveMessage(command.ToString());

            if (this.secondsRemainingUiController.InvervalInSeconds == timeToAnswerInSeconds)
            {
                IntegrationTest.Pass();
            }
            else
            {
                IntegrationTest.Fail();
            }
        }
    }
}