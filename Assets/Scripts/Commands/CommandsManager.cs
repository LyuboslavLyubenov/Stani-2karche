using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class CommandsManager
{
    Dictionary<string, List<INetworkManagerCommand>> commands = new Dictionary<string, List<INetworkManagerCommand>>();

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

        if (!commands.ContainsKey(commandName))
        {
            var commandsToExecute = new List<INetworkManagerCommand>();
            commands.Add(commandName, commandsToExecute);
        }

        commands[commandName].Add(commandToExecute);
    }

    public void RemoveCommand(string commandName)
    {
        if (!commands.ContainsKey(commandName))
        {
            throw new ArgumentException("Command " + commandName + " cannot be found");
        }

        commands.Remove(commandName);
    }

    public void Execute(NetworkCommandData command)
    {
        var commandName = command.Name;

        if (!commands.ContainsKey(commandName))
        {
            throw new ArgumentException("Command not found");
        }

        var commandsToExecute = commands[commandName];

        for (int i = 0; i < commandsToExecute.Count; i++)
        {
            var commandToExecute = commandsToExecute[i];
            commandToExecute.Execute(command.Options);    
        }
    }
}
