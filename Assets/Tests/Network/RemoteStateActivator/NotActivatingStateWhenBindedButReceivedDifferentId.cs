﻿using DummyClientNetworkManager = Tests.DummyObjects.DummyClientNetworkManager;

namespace Assets.Tests.Network.RemoteStateActivator
{
    using Assets.Scripts.Interfaces.Network;
    using Assets.Tests.DummyObjects.States.EveryBodyVsTheTeacher;

    using Commands;

    using Interfaces.Network.NetworkManager;

    using StateMachine;

    using UnityEngine;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Zenject;

    public class NotActivatingStateWhenBindedButReceivedDifferentId : MonoBehaviour
    {
        [Inject]
        private IClientNetworkManager networkManager;

        [Inject]
        private StateMachine stateMachine;

        [Inject]
        private IRemoteStateActivator stateActivator;
        
        void Start()
        {
            this.networkManager.CommandsManager.RemoveAllCommands();

            var dummyState = new DummyRoundState();
            this.stateActivator.Bind("TestState", dummyState);

            var dummyClientNetworkManager = (DummyClientNetworkManager)this.networkManager;
            var activateStateCommand = new NetworkCommandData("ActivateNotThisTestState");
            dummyClientNetworkManager.FakeReceiveMessage(activateStateCommand.ToString());

            if (this.stateMachine.CurrentState != dummyState)
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