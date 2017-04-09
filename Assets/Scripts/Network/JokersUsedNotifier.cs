namespace Network
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Commands;

    using EventArgs;

    using Interfaces;
    using Interfaces.Network;
    using Interfaces.Network.NetworkManager;

    class SelectedJokerCommand : INetworkManagerCommand
    {
        public event EventHandler<JokerTypeEventArgs> OnSelected = delegate
            { };

        private readonly Type jokerType;

        public SelectedJokerCommand(Type jokerType)
        {
            if (!jokerType.GetInterfaces().Contains(typeof(IJoker)))
            {
                throw new ArgumentException();
            }

            this.jokerType = jokerType;
        }

        public void Execute(Dictionary<string, string> commandsOptionsValues)
        {
            this.OnSelected(this, new JokerTypeEventArgs(this.jokerType));
        }
    }

    public class JokersUsedNotifier : IJokersUsedNotifier
    {
        public event EventHandler<JokerTypeEventArgs> OnUsedJoker = delegate { };

        private readonly IServerNetworkManager networkManager;
        private readonly JokersData jokersData;

        private readonly Dictionary<Type, INetworkManagerCommand> trackedJokersUsedCommands = new Dictionary<Type, INetworkManagerCommand>();

        public JokersUsedNotifier(IServerNetworkManager networkManager, JokersData jokersData)
        {
            if (networkManager == null)
            {
                throw new ArgumentNullException("networkManager");
            }

            if (jokersData == null)
            {
                throw new ArgumentNullException("jokersData");
            }

            this.networkManager = networkManager;
            this.jokersData = jokersData;

            jokersData.OnAddedJoker += this.OnAddedJoker;
            jokersData.OnRemovedJoker += this.OnRemovedJoker;
        }
        
        private void OnAddedJoker(object sender, JokerTypeEventArgs args)
        {
            if (this.trackedJokersUsedCommands.ContainsKey(args.JokerType))
            {
                return;
            }

            var onUsedJokerCommand = new SelectedJokerCommand(args.JokerType);

            onUsedJokerCommand.OnSelected += this.OnJokerUsedCommandExecuted;

            var commandName = "Selected" + args.JokerType.Name;
            this.networkManager.CommandsManager.AddCommand(commandName, onUsedJokerCommand);
            this.trackedJokersUsedCommands.Add(args.JokerType, onUsedJokerCommand);
        }
        
        private void OnJokerUsedCommandExecuted(object sender, JokerTypeEventArgs args)
        {
            var jokerName = args.JokerType.Name.Replace("Selected", "");
            var jokerType = this.jokersData.AvailableJokers.First(j => j.Name == jokerName);
            this.OnUsedJoker(this, new JokerTypeEventArgs(jokerType));
        }

        private void OnRemovedJoker(object sender, JokerTypeEventArgs args)
        {
            if (!this.trackedJokersUsedCommands.ContainsKey(args.JokerType))
            {
                return;
            }

            var onUsedJokerCommand = this.trackedJokersUsedCommands[args.JokerType];

            if (this.networkManager.CommandsManager.Exists(onUsedJokerCommand))
            {
                ((SelectedJokerCommand)onUsedJokerCommand).OnSelected -= this.OnJokerUsedCommandExecuted;
                this.networkManager.CommandsManager.RemoveCommand(onUsedJokerCommand);
            }

            this.trackedJokersUsedCommands.Remove(args.JokerType);
        }

        public void Dispose()
        {
            jokersData.OnAddedJoker -= this.OnAddedJoker;
            jokersData.OnRemovedJoker -= this.OnRemovedJoker;
        }
    }
}