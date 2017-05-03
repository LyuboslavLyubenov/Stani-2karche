﻿using DummyClientNetworkManager = Tests.DummyObjects.DummyClientNetworkManager;
using DummyServerNetworkManager = Tests.DummyObjects.DummyServerNetworkManager;
using ExtendedMonoBehaviour = Utils.Unity.ExtendedMonoBehaviour;

namespace Assets.Tests.States.Presenter.Playing
{
    using Assets.Scripts.Interfaces.Controllers;
    using Assets.Scripts.States.EveryBodyVsTheTeacher.Presenter;

    using Commands;

    using Interfaces.Network.NetworkManager;

    using Jokers;

    using StateMachine;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Zenject.Source.Usage;

    public class RemoveUsedJoker : ExtendedMonoBehaviour
    {
        [Inject]
        private IClientNetworkManager networkManager;

        [Inject]
        private IAvailableJokersUIController availableJokersUiController;

        [Inject]
        private StateMachine stateMachine;

        [Inject]
        private PlayingState state;

        void Start()
        {
            this.stateMachine.SetCurrentState(this.state);

            var dummyServerNetworkManager = (DummyClientNetworkManager)this.networkManager;

            var addJokerCommand = new NetworkCommandData("Add" + typeof(HelpFromFriendJoker).Name);
            dummyServerNetworkManager.FakeReceiveMessage(addJokerCommand.ToString());

            this.CoroutineUtils.WaitForSeconds(0.5f,
                () =>
                    {
                        var removeJokerCommand = new NetworkCommandData("Remove" + typeof(HelpFromFriendJoker).Name);
                        dummyServerNetworkManager.FakeReceiveMessage(removeJokerCommand.ToString());

                        if (this.availableJokersUiController.JokersCount == 0)
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