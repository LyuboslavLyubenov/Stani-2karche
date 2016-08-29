using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class CommandsManager
{
    Dictionary<string, INetworkManagerCommand> commandsToExecute = new Dictionary<string, INetworkManagerCommand>();

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

        commandsToExecute.Add(commandName, commandToExecute);
    }

    public void Execute(NetworkCommandData command)
    {
        var commandName = command.Name;

        if (!commandsToExecute.ContainsKey(commandName))
        {
            throw new ArgumentException("Command not found");
        }

        var commandToExecute = commandsToExecute[commandName];
        commandToExecute.Execute(command.Options);
    }
}
