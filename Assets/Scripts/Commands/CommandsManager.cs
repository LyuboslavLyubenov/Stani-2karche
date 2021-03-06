﻿namespace Commands
{

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Interfaces.Network.NetworkManager;

    public class CommandsManager : ICommandsManager
    {
        private Dictionary<string, List<INetworkManagerCommand>> commands = new Dictionary<string, List<INetworkManagerCommand>>();

        public void AddCommand(INetworkManagerCommand commandToExecute)
        {
            var commandName = commandToExecute.GetType().Name.Replace("Command", "");
            this.AddCommand(commandName, commandToExecute);
        }

        public void AddCommand(string commandName, INetworkManagerCommand commandToExecute)
        {
            if (string.IsNullOrEmpty(commandName))
            {
                throw new ArgumentException("commandName cannot be empty");
            }

            if (commandToExecute == null)
            {
                throw new ArgumentNullException("commandToExecute");
            }

            if (!this.commands.ContainsKey(commandName))
            {
                var commandsToExecute = new List<INetworkManagerCommand>();
                this.commands.Add(commandName, commandsToExecute);
            }

            this.commands[commandName].Add(commandToExecute);
        }

        public void AddCommands(IEnumerable<INetworkManagerCommand> commands)
        {
            if (commands == null || !commands.Any())
            {
                throw new ArgumentNullException("commands");
            }

            commands.ToList().ForEach(this.AddCommand);
        }

        public bool Exists(INetworkManagerCommand command)
        {
            var commandsKeyValuePairs = this.commands.ToList();

            for (int i = 0; i < commandsKeyValuePairs.Count; i++)
            {
                var commands = commandsKeyValuePairs[i].Value;
                
                if (commands.Contains(command))
                {
                    return true;
                }
            }

            return false;
        }

        public bool Exists<T>()
        {
            var commandName = typeof(T).Name.Replace("Command", "");
            return this.Exists(commandName);
        }

        public bool Exists(string commandName)
        {
            return this.commands.ContainsKey(commandName);
        }

        public void RemoveCommand<T>()
        {
            var commandName = typeof(T).Name.Replace("Command", "");
            this.RemoveCommand(commandName);
        }

        public void RemoveCommand(string commandName)
        {
            if (!this.commands.ContainsKey(commandName))
            {
                throw new ArgumentException("Command " + commandName + " cannot be found");
            }
            
            this.commands.Remove(commandName);
        }

        public void RemoveCommand(INetworkManagerCommand command)
        {
            if (!this.Exists(command))
            {
                throw new ArgumentException("Command not added");
            }

            var commandKey = this.commands.First(c => c.Value.Contains(command))
                .Key;
            
            this.commands[commandKey].Remove(command);

            if (this.commands[commandKey].Count == 0)
            {
                this.commands.Remove(commandKey);
            }
        }

        public void RemoveAllCommands()
        {
            this.commands.Clear();
        }

        public void Execute(NetworkCommandData command)
        {
            var commandName = command.Name;

            if (!this.commands.ContainsKey(commandName))
            {
                throw new ArgumentException("Command with name " + command.Name + " not found");
            }

            var commandsToExecute = this.commands[commandName];

            for (int i = 0; i < commandsToExecute.Count; i++)
            {
                var commandToExecute = commandsToExecute[i];
                commandToExecute.Execute(command.Options);    

                var oneTimeExecuteCommand = commandToExecute as IOneTimeExecuteCommand;

                if (oneTimeExecuteCommand != null && 
                    oneTimeExecuteCommand.FinishedExecution &&
                    this.Exists(commandName))
                {
                    this.commands[commandName].Remove(oneTimeExecuteCommand);
                }
            }
        }
    }
}