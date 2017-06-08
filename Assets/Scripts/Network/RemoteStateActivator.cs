using IClientNetworkManager = Interfaces.Network.NetworkManager.IClientNetworkManager;

namespace Assets.Scripts.Network
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Assets.Scripts.Commands;
    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Interfaces.Network;

    using StateMachine;

    public class RemoteStateActivator : IRemoteStateActivator
    {
        private readonly IClientNetworkManager networkManager;

        private readonly StateMachine stateMachine;

        private readonly HashSet<string> bindedStatesCommandNames = new HashSet<string>();
 
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
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException("id");
            }

            if (stateToActivate == null)
            {
                throw new ArgumentNullException("stateToActivate");
            }

            if (this.networkManager.CommandsManager.Exists("Activate" + id))
            {
                throw new InvalidOperationException("Id already used");
            }

            var commandName = "Activate" + id;
            var activateStateCommand = new ActivateStateCommand(this.stateMachine, stateToActivate);
            this.networkManager.CommandsManager.AddCommand(commandName, activateStateCommand);
            this.bindedStatesCommandNames.Add(commandName);
        }

        public void UnBind(string id)
        {
            var commandName = "Activate" + id;

            if (!this.networkManager.CommandsManager.Exists(commandName))
            {
                throw new ArgumentException("There arent any states associated with " + id);
            }
            
            this.networkManager.CommandsManager.RemoveCommand(commandName);
            this.bindedStatesCommandNames.Remove(commandName);
        }

        public void Dispose()
        {
            var bindedStatesCommandNames = this.bindedStatesCommandNames.ToArray();

            for (int i = 0; i < bindedStatesCommandNames.Length; i++)
            {
                var commandName = bindedStatesCommandNames[i];
                if (this.networkManager.CommandsManager.Exists(commandName))
                {
                    this.networkManager.CommandsManager.RemoveCommand(commandName);
                }

                this.bindedStatesCommandNames.Remove(commandName);
            }

            this.bindedStatesCommandNames.Clear();
        }
    }
}