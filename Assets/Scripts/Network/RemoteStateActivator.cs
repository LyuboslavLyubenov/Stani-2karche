using IClientNetworkManager = Interfaces.Network.NetworkManager.IClientNetworkManager;

namespace Assets.Scripts.Network
{

    using System;

    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Interfaces.Network;

    using StateMachine;

    public class RemoteStateActivator : IRemoteStateActivator
    {
        private readonly IClientNetworkManager networkManager;

        private readonly StateMachine stateMachine;

        public RemoteStateActivator(IClientNetworkManager networkManager, StateMachine stateMachine)
        {
            if (networkManager == null)
            {
                throw new ArgumentNullException("networkManager");
            }

            if (stateMachine == null)
            {
                throw new ArgumentNullException("stateMachine");
            }

            this.networkManager = networkManager;
            this.stateMachine = stateMachine;
        }

        public void Bind(string id, IState stateToActivate)
        {
            throw new System.NotImplementedException();
        }

        public void UnBind(string id)
        {
            throw new System.NotImplementedException();
        }

        public void UnBind(IState state)
        {
            throw new System.NotImplementedException();
        }

        public void Dispose()
        {
            throw new System.NotImplementedException();
        }

    }
}
