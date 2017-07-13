using DummyClientNetworkManager = Tests.DummyObjects.DummyClientNetworkManager;

namespace Assets.Tests.Jokers.KalitkoJoker
{

    using Assets.Scripts.Commands.Jokers.Result;
    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Interfaces.Controllers;
    using Assets.Tests.DummyObjects.UIControllers;

    using Commands;

    using Interfaces.Network.NetworkManager;

    using UnityEngine;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Zenject;

    public class ReceivedNothing : MonoBehaviour
    {
        [Inject]
        private IClientNetworkManager networkManager;
       
        [Inject]
        private IKalitkoJokerUIController kalitkoJokerUIController;

        [Inject]
        private IJoker joker;

        void Start()
        {
            this.joker.Activate();

            var dummyKalitkoJokerUIController = (DummyKalitkoJokerUIContainer)this.kalitkoJokerUIController;
            dummyKalitkoJokerUIController.OnShowAnswer += (sender, args) => IntegrationTest.Fail();
            dummyKalitkoJokerUIController.OnShowNothing += (sender, args) => IntegrationTest.Pass();

            var dummyClientNetworkManager = (DummyClientNetworkManager)this.networkManager;
            var jokerResultCommand = NetworkCommandData.From<KalitkoJokerResultCommand>();
            dummyClientNetworkManager.FakeReceiveMessage(jokerResultCommand.ToString());
        }
    }

}