using IClientNetworkManager = Interfaces.Network.NetworkManager.IClientNetworkManager;

namespace Assets.Scripts.States.EveryBodyVsTheTeacher.Presenter
{

    using System;

    using Interfaces;
    using StateMachine;
    
    public class PlayingState : IState
    {
        private readonly IClientNetworkManager networkManager;

        public PlayingState(IClientNetworkManager networkManager)
        {
            if (networkManager == null)
            {
                throw new ArgumentNullException("networkManager");
            }

            this.networkManager = networkManager;
        }

        public void OnStateEnter(StateMachine stateMachine)
        {

        }

        public void OnStateExit(StateMachine stateMachine)
        {
            throw new System.NotImplementedException();
        }
    }
}
