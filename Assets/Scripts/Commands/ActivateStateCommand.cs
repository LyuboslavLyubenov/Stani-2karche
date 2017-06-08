using INetworkManagerCommand = Interfaces.Network.NetworkManager.INetworkManagerCommand;

namespace Assets.Scripts.Commands
{

    using System;
    using System.Collections.Generic;

    using Assets.Scripts.Interfaces;

    using StateMachine;

    public class ActivateStateCommand : INetworkManagerCommand
    {
        private readonly StateMachine stateMachine;
        private readonly IState state;

        public ActivateStateCommand(StateMachine stateMachine, IState state)
        {
            if (stateMachine == null)
            {
                throw new ArgumentNullException("stateMachine");
            }

            if (state == null)
            {
                throw new ArgumentNullException("state");
            }

            this.stateMachine = stateMachine;
            this.state = state;
        }

        public void Execute(Dictionary<string, string> commandsOptionsValues)
        {
            this.stateMachine.SetCurrentState(this.state);
        }
    }
}
