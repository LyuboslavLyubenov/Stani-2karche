using DummyClientNetworkManager = Tests.DummyObjects.DummyClientNetworkManager;

namespace Assets.Tests.Jokers.ConsultWithTeacherJoker
{

    using Assets.Scripts.Interfaces;

    using Commands;

    using Interfaces;
    using Interfaces.Network.NetworkManager;

    using UnityEngine;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Zenject.Source.Usage;

    public class WhenActivatedShowLoadingScreenUntilReceivedSettings : MonoBehaviour
    {
        [Inject]
        private IClientNetworkManager networkManager;
        
        [Inject]
        private ISimpleQuestion question;
        
        [Inject(Id = "LoadingUI")]
        private GameObject loadingUI;

        [Inject]
        private IJoker joker;

        void Start()
        {
            this.joker.Activate();

            if (this.loadingUI.activeSelf)
            {
                IntegrationTest.Pass();
            }
            else
            {
                IntegrationTest.Fail();
            }

            var dummyClientNetworkManager = (DummyClientNetworkManager)this.networkManager;
            var settingsCommand = new NetworkCommandData("ConsultWithTeacherJokerSettings");
            settingsCommand.AddOption("AnswersToDisable", "Asd");
            dummyClientNetworkManager.FakeReceiveMessage(settingsCommand.ToString());

            if (this.loadingUI.activeSelf)
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