using DummyClientNetworkManager = Tests.DummyObjects.DummyClientNetworkManager;

namespace Assets.Tests.Network.RemoteStateActivator
{

    using Assets.Scripts.Interfaces.Network;

    using Commands;

    using Interfaces.Network.NetworkManager;

    using StateMachine;

    using UnityEngine;

    using Zenject.Source.Usage;

    public class NotActivatingAnyStateIfNoneWereBinded : MonoBehaviour
    {
        [Inject]
        private IClientNetworkManager networkManager;

        [Inject]
        private StateMachine stateMachine;

        [Inject]
        private IRemoteStateActivator stateActivator;

        void Start()
        {
            var dummyClientNetworkManager = (DummyClientNetworkManager)this.networkManager;
            var activateStateCommand = new NetworkCommandData("ActivateNotBindedState");
            dummyClientNetworkManager.FakeReceiveMessage(activateStateCommand.ToString());
        }
    }
}