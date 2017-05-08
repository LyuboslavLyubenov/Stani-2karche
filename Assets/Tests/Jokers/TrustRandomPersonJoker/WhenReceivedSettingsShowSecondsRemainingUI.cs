using DummyClientNetworkManager = Tests.DummyObjects.DummyClientNetworkManager;

namespace Assets.Tests.Jokers.TrustRandomPersonJoker
{
    using UnityEngine;

    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Interfaces.Controllers;

    using Commands;

    using Interfaces.Network.NetworkManager;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Zenject.Source.Usage;

    public class WhenReceivedSettingsShowSecondsRemainingUI : MonoBehaviour
    {
        [Inject]
        private IClientNetworkManager networkManager;

        [Inject]
        private IJoker joker;

        [Inject]
        private GameObject secondsRemainingUI;

        [Inject]
        private int seconds;

        [Inject]
        private ISecondsRemainingUIController secondsRemainingUiController;

        void Start()
        {
            this.joker.Activate();

            var dummyClientNetworkManager = (DummyClientNetworkManager)this.networkManager;
            var settingsCommand = new NetworkCommandData("TrustRandomPersonJokerSettings");
            settingsCommand.AddOption("TimeToAnswerInSeconds", this.seconds.ToString());
            dummyClientNetworkManager.FakeReceiveMessage(settingsCommand.ToString());

            if (this.secondsRemainingUI.activeSelf &&
                this.secondsRemainingUiController.Running && 
                this.secondsRemainingUiController.InvervalInSeconds == this.seconds)
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