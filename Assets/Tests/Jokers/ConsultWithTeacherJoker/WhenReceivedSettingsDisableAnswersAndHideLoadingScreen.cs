using DummyClientNetworkManager = Tests.DummyObjects.DummyClientNetworkManager;
using ExtendedMonoBehaviour = Utils.Unity.ExtendedMonoBehaviour;

namespace Assets.Tests.Jokers.ConsultWithTeacherJoker
{
    using System.Collections.Generic;
    using System.Linq;

    using Assets.Scripts.Interfaces;
    using Assets.Tests.DummyObjects.UIControllers;

    using Commands;

    using Interfaces;
    using Interfaces.Controllers;
    using Interfaces.Network.NetworkManager;

    using UnityEngine;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Zenject.Source.Usage;

    public class WhenReceivedSettingsDisableAnswersAndHideLoadingScreen : ExtendedMonoBehaviour
    {
        [Inject]
        private IClientNetworkManager networkManager;

        [Inject]
        private ISimpleQuestion question;

        [Inject(Id = "LoadingUI")]
        private GameObject loadingUI;

        [Inject(Id = "ElectionQuestionUI")]
        private GameObject ElectionQuestionUI;

        [Inject]
        private IElectionQuestionUIController electionQuestionUIController;

        [Inject]
        private IJoker joker;

        void Start()
        {
            this.joker.Activate();

            var hiddenAnswers = new List<string>();
            var dummyElectionQuestionUIController =
                (DummyElectionQuestionUIController)this.electionQuestionUIController;
            dummyElectionQuestionUIController.OnHideAnswer += (sender, args) =>
                {
                    hiddenAnswers.Add(args.Answer);
                };

            var dummyClientNetworkManager = (DummyClientNetworkManager)this.networkManager;
            var settingsCommand = new NetworkCommandData("ConsultWithTeacherJokerSettings");
            var wrongAnswers = this.question.Answers.Where(a => a != this.question.CorrectAnswer)
                .Take(2)
                .ToArray();
            settingsCommand.AddOption("AnswersToDisable", string.Join(", ", wrongAnswers));
            dummyClientNetworkManager.FakeReceiveMessage(settingsCommand.ToString());

            this.CoroutineUtils.WaitForFrames(1,
                () =>
                    {
                        if (this.ElectionQuestionUI.activeSelf && !hiddenAnswers.Except(wrongAnswers).Any())
                        {
                            IntegrationTest.Pass();
                        }
                        else
                        {
                            IntegrationTest.Fail();
                        }
                    });
        }
    }
}