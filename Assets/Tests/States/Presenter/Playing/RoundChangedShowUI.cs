using DummyClientNetworkManager = Tests.DummyObjects.DummyClientNetworkManager;

namespace Assets.Tests.States.Presenter.Playing
{

    using Assets.Scripts.Interfaces.Controllers;
    using Assets.Scripts.States.EveryBodyVsTheTeacher.Presenter;

    using Commands;

    using Interfaces.Network.NetworkManager;

    using StateMachine;

    using UnityEngine;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Zenject.Source.Usage;

    public class RoundChangedShowUI : MonoBehaviour
    {
        [Inject]
        private IClientNetworkManager networkManager;

        [Inject]
        private IChangedRoundUIController changedRoundUiController;

        [Inject]
        private StateMachine stateMachine;

        [Inject]
        private PlayingState state;

        void Start()
        {
            this.stateMachine.SetCurrentState(this.state);

            var dummyNetworkManager = (DummyClientNetworkManager)this.networkManager;
            var roundChangedCommand = new NetworkCommandData("SwitchedToNextRound");
            roundChangedCommand.AddOption("Round", "2");
            dummyNetworkManager.FakeReceiveMessage(roundChangedCommand.ToString());

            if (this.changedRoundUiController.Round == 2)
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