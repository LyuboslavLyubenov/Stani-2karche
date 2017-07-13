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

    public class ReceivedAnswer : MonoBehaviour
    {
        [Inject]
        private IClientNetworkManager networkManager;

        [Inject]
        private IKalitkoJokerUIController kalitkoJokerUIController;

        [Inject]
        private string answer;

        [Inject]
        private IJoker joker;

        void Start()
        {
            this.joker.Activate();
            
            var dummyKalitkoJokerUIController = (DummyKalitkoJokerUIContainer)this.kalitkoJokerUIController;
            dummyKalitkoJokerUIController.OnShowAnswer += (sender, args) =>
                {
                    if (args.Answer == this.answer)
                    {
                        IntegrationTest.Pass();
                    }
                    else
                    {
                        IntegrationTest.Fail();
                    }
                };
            dummyKalitkoJokerUIController.OnShowNothing += (sender, args) => IntegrationTest.Fail();

            var dummyClientNetworkManager = (DummyClientNetworkManager)this.networkManager;
            var jokerResultCommand = NetworkCommandData.From<KalitkoJokerResultCommand>();
            jokerResultCommand.AddOption("Answer", this.answer);
            dummyClientNetworkManager.FakeReceiveMessage(jokerResultCommand.ToString());
        }
    }

}