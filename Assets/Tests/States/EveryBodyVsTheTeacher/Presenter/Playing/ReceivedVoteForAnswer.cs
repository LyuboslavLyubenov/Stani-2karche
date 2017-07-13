﻿using DummyClientNetworkManager = Tests.DummyObjects.DummyClientNetworkManager;

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

    public class ReceivedVoteForAnswer : MonoBehaviour
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

            var dummyServerNetworkManager = (DummyClientNetworkManager)this.networkManager;
            var questionJSON = JsonUtility.ToJson(this.question.Serialize());
            var timeToAnswerInSeconds = 10;

            var command = new NetworkCommandData("LoadQuestion");
            command.AddOption("QuestionJSON", questionJSON);
            command.AddOption("TimeToAnswer", timeToAnswerInSeconds.ToString());
            dummyServerNetworkManager.FakeReceiveMessage(command.ToString());

            var answerSelectedCommand = new NetworkCommandData("AnswerSelected");
            answerSelectedCommand.AddOption("ConnectionId", "1");
            answerSelectedCommand.AddOption("Answer", this.question.CorrectAnswer);
            dummyServerNetworkManager.FakeReceiveMessage(answerSelectedCommand.ToString());

            if (this.electionQuestionUiController.HighestVotedAnswer == this.question.CorrectAnswer)
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