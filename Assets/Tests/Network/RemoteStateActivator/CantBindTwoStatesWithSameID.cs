namespace Assets.Tests.Network.RemoteStateActivator
{

    using Assets.Scripts.Interfaces.Network;
    using Assets.Tests.DummyObjects.States.EveryBodyVsTheTeacher;

    using Interfaces.Network.NetworkManager;

    using UnityEngine;

    using Zenject;

    public class CantBindTwoStatesWithSameID : MonoBehaviour
    {
        [Inject]
        private IClientNetworkManager networkManager;

        [Inject]
        private IRemoteStateActivator stateActivator;

        void Start()
        {
            this.networkManager.CommandsManager.RemoveAllCommands();

            this.stateActivator.Bind("State", new DummyRoundState());
            this.stateActivator.Bind("State", new DummyRoundState());
        }
    }
}