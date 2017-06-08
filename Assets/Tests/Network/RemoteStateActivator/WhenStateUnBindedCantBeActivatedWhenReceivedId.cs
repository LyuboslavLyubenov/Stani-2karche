using DummyClientNetworkManager = Tests.DummyObjects.DummyClientNetworkManager;
using ExtendedMonoBehaviour = Utils.Unity.ExtendedMonoBehaviour;

namespace Assets.Tests.Network.RemoteStateActivator
{

    using Assets.Scripts.Interfaces.Network;
    using Assets.Tests.DummyObjects.States.EveryBodyVsTheTeacher;

    using Commands;

    using Interfaces.Network.NetworkManager;

    using StateMachine;

    using UnityEngine;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Zenject.Source.Usage;

    public class WhenStateUnBindedCantBeActivatedWhenReceivedId : ExtendedMonoBehaviour
    {
        [Inject]
        private IClientNetworkManager networkManager;

        [Inject]
        private IRemoteStateActivator stateActivator;

        [Inject]
        private StateMachine stateMachine;

        void Start()
        {
            var dummyState = new DummyRoundState();
            this.stateActivator.Bind("Test", dummyState);

            this.CoroutineUtils.WaitForFrames(1,
                () =>
                    {
                        this.stateActivator.UnBind("Test");

                        var dummyClientNetworkManager = (DummyClientNetworkManager)this.networkManager;
                        var activateStateCommand = new NetworkCommandData("ActivateState");
                        activateStateCommand.AddOption("Id", "Test");
                        dummyClientNetworkManager.FakeReceiveMessage(activateStateCommand.ToString());

                        if (this.stateMachine.CurrentState != dummyState)
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

    public class WhenStateUnBindedCantBeActivatedWhenReceivedId2 : ExtendedMonoBehaviour
    {
        [Inject]
        private IClientNetworkManager networkManager;

        [Inject]
        private IRemoteStateActivator stateActivator;

        [Inject]
        private StateMachine stateMachine;

        void Start()
        {
            var dummyState = new DummyRoundState();
            this.stateActivator.Bind("Test", dummyState);

            this.CoroutineUtils.WaitForFrames(1,
                () =>
                    {
                        this.stateActivator.UnBind(dummyState);

                        var dummyClientNetworkManager = (DummyClientNetworkManager)this.networkManager;
                        var activateStateCommand = new NetworkCommandData("ActivateState");
                        activateStateCommand.AddOption("Id", "Test");
                        dummyClientNetworkManager.FakeReceiveMessage(activateStateCommand.ToString());

                        if (this.stateMachine.CurrentState != dummyState)
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