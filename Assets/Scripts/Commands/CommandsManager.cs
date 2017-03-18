﻿namespace Commands
{

    using System;
    using System.Collections.Generic;

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
            var commandName = command.GetType()
                .Name.Replace("Command", "");

            if (!this.Exists(commandName))
            {
                throw new ArgumentException("Command doesnt exists");
            }

            this.commands[commandName].Remove(command);
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