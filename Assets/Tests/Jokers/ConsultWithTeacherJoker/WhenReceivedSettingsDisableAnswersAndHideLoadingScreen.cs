using DummyClientNetworkManager = Tests.DummyObjects.DummyClientNetworkManager;

namespace Assets.Tests.Jokers.ConsultWithTeacherJoker
{
    using System.Linq;

    using Assets.Scripts.Interfaces;

    using Commands;

    using Interfaces;
    using Interfaces.Controllers;
    using Interfaces.Network.NetworkManager;

    using UnityEngine;

    using Zenject.Source.Usage;

    public class WhenReceivedSettingsDisableAnswersAndHideLoadingScreen : MonoBehaviour
    {
        [Inject]
        private IClientNetworkManager networkManager;

        [Inject]
        private ISimpleQuestion question;

        [Inject]
        private GameObject loadingUI;

        [Inject]
        private IElectionQuestionUIController electionQuestionUIController;

        [Inject]
        private IJoker joker;

        void Start()
        {
            this.joker.Activate();

            var dummyClientNetworkManager = (DummyClientNetworkManager)this.networkManager;
            var settingsCommand = new NetworkCommandData("ConsultWithTeacherJokerSettings");
            var wrongAnswers = this.question.Answers.Where(a => a != this.question.CorrectAnswer)
                .Take(2)
                .ToArray();
            settingsCommand.AddOption("AnswersToDisable", string.Join(", ", wrongAnswers));
            dummyClientNetworkManager.FakeReceiveMessage(settingsCommand.ToString());

            for (int i = 0; i < wrongAnswers.Length; i++)
            {
                var answer = wrongAnswers[i];
                this.electionQuestionUIController.HideAnswer(answer);
            }
        }
    }
}