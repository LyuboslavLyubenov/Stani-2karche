using DummyClientNetworkManager = Tests.DummyObjects.DummyClientNetworkManager;
using ExtendedMonoBehaviour = Utils.Unity.ExtendedMonoBehaviour;

namespace Assets.Tests.Jokers.TrustRandomPersonJoker
{
    using UnityEngine;

    using Assets.Scripts.Interfaces;

    using Commands;

    using Interfaces.Network.NetworkManager;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Zenject;

    public class WhenReceivedAnswerTimeoutShowNotReceivedAnswerUI : ExtendedMonoBehaviour
    {
        [Inject]
        private IClientNetworkManager networkManager;

        [Inject]
        private GameObject notReceivedAnswerUI;

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
            dummyClientNetworkManager.FakeReceiveMessage(settingsCommand.ToString());

            this.CoroutineUtils.WaitForFrames(
                10,
                () =>
                    {
                        var answerTimeoutCommand = new NetworkCommandData("AnswerTimeout");
                        dummyClientNetworkManager.FakeReceiveMessage(answerTimeoutCommand.ToString());

                        if (this.notReceivedAnswerUI.activeSelf)
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