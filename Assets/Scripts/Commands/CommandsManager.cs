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

    public void Execute(string command)
    {
        if (string.IsNullOrEmpty(command))
        {
            throw new ArgumentNullException("command");
        }

        var commandDelitemerIndex = command.IndexOf('-');

        if (commandDelitemerIndex < 0)
        {
            throw new ArgumentException("Invalid command");
        }

        var commandName = command.Substring(0, commandDelitemerIndex);

        if (!commandsToExecute.ContainsKey(commandName))
        {
            throw new Exception("Command " + commandName + " not found.");
        }

        var commandOptionsValues = new Dictionary<string, string>();

        if (commandDelitemerIndex < command.Length)
        {
            var commandOptions = command.Substring(commandDelitemerIndex + 1)
                .Skip(1)
                .ToString().Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                .ToArray();

            foreach (var option in commandOptions)
            {
                var optionValueDelitemer = option.IndexOf('=');

                if (optionValueDelitemer <= 0 || option.Length <= (optionValueDelitemer + 1))
                {
                    return;
                }

                var optionName = option.Substring(0, optionValueDelitemer);
                var optionValue = option.Substring(optionValueDelitemer + 1);

                commandOptionsValues.Add(optionName, optionValue);
            }
        }
 
        var commandToExecute = commandsToExecute[commandName];
        commandToExecute.Execute(commandOptionsValues);
    }
}
