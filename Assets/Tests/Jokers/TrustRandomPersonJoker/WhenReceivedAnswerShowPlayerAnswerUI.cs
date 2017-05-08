using DummyClientNetworkManager = Tests.DummyObjects.DummyClientNetworkManager;
using ExtendedMonoBehaviour = Utils.Unity.ExtendedMonoBehaviour;

namespace Assets.Tests.Jokers.TrustRandomPersonJoker
{
    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Interfaces.Controllers;

    using Commands;

    using Interfaces;
    using Interfaces.Network.NetworkManager;

    using UnityEngine;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Zenject.Source.Usage;

    public class WhenReceivedAnswerShowPlayerAnswerUI : ExtendedMonoBehaviour
    {
        [Inject]
        private IClientNetworkManager networkManager;

        [Inject]
        private ISimpleQuestion question;

        [Inject]
        private GameObject resultUI;

        [Inject]
        private IPlayerAnswerUIController resultUIController;

        [Inject]
        private int seconds;

        [Inject]
        private IJoker joker;
       
        void Start()
        {
            this.joker.Activate();

            var dummyClientNetworkManager = (DummyClientNetworkManager)this.networkManager;
            
            var settingsCommand = new NetworkCommandData("TrustRandomPersonJokerSettings");
            settingsCommand.AddOption("TimeToAnswerInSeconds", this.seconds.ToString());

            var username = "Ivan";
            var answer = this.question.CorrectAnswer;
            var answerCommand = new NetworkCommandData("TrustRandomAnswerJokerResult");
            answerCommand.AddOption("Username", username);
            answerCommand.AddOption("Answer", answer);
            dummyClientNetworkManager.FakeReceiveMessage(answerCommand.ToString());

            this.CoroutineUtils.WaitForFrames(2,
                () =>
                    {
                        if (this.resultUI.activeSelf &&
                            this.resultUIController.Username == username &&
                            this.resultUIController.Answer == answer)
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