using AskAudienceJoker = Jokers.AskAudienceJoker;
using NetworkCommandData = Commands.NetworkCommandData;

namespace Tests.States.Presenter.Playing
{

    using Assets.Scripts.Interfaces.Controllers.EveryBodyVsTheTeacher.Presenter;
    using Assets.Scripts.States.EveryBodyVsTheTeacher.Presenter;

    using Interfaces.Network.NetworkManager;

    using StateMachine;

    using Tests.DummyObjects;

    using UnityEngine;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Zenject.Source.Usage;

    public class AddJoker : MonoBehaviour
    {
        [Inject]
        private IClientNetworkManager networkManager;
        
        [Inject]
        private StateMachine stateMachine;

        [Inject]
        private IAvailableJokersUIController availableJokersUiController;
        
        [Inject]
        private PlayingState state;
        
        void Start()
        {
            this.stateMachine.SetCurrentState(this.state);

            var dummyClientNetworkManager = (DummyClientNetworkManager)this.networkManager;
            var addJokerCommand = new NetworkCommandData("Add" + typeof(AskAudienceJoker));
            dummyClientNetworkManager.FakeReceiveMessage(addJokerCommand.ToString());

            if (this.availableJokersUiController.JokersCount == 1)
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