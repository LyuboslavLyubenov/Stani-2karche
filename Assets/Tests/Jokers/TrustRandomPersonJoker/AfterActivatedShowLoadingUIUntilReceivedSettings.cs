using DummyClientNetworkManager = Tests.DummyObjects.DummyClientNetworkManager;

namespace Assets.Tests.Jokers.TrustRandomPersonJoker
{
    using Assets.Scripts.Interfaces;
    using Commands;
    using Interfaces.Network.NetworkManager;
    using UnityEngine;
    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Zenject;

    public class AfterActivatedShowLoadingUIUntilReceivedSettings : MonoBehaviour
    {
        [Inject]
        private IClientNetworkManager networkManager;

        [Inject]
        private GameObject LoadingUI;

        [Inject]
        private int seconds;

        [Inject]
        private IJoker joker;

        void Start()
        {
            this.joker.Activate();

            if (this.LoadingUI.activeSelf)
            {
                IntegrationTest.Pass();
            }
            else
            {
                IntegrationTest.Fail();
            }

            var dummyClientNetworkManager = (DummyClientNetworkManager)this.networkManager;
            var settingsCommand = new NetworkCommandData("TrustRandomPersonJokerSettings");
            settingsCommand.AddOption("TimeToAnswerInSeconds", this.seconds.ToString());
            dummyClientNetworkManager.FakeReceiveMessage(settingsCommand.ToString());

            if (this.LoadingUI.activeSelf)
            {
                IntegrationTest.Fail();
            }
            else
            {
                IntegrationTest.Pass();
            }
        }
    }

}