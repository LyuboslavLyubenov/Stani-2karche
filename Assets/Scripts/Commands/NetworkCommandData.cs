using System.Collections.Generic;
using System.Text;
using System.Linq;
using System;

public class NetworkCommandData
{
    //TODO: Move in external class
    public const int CODE_OptionClientConnectionIdValueAll = -1;
    public const int CODE_OptionClientConnectionIdValueRandom = -2;

    const int MinCommandNameLength = 3;
    const int MinOptionNameLength = 3;
    const int MinOptionValueLength = 1;
    const string CommandTag = "[COMMAND]";
    const char OptionsDelitemerSymbol = ';';
    const char OptionsBeginDelitemerSymbol = '-';
    const char OptionValueDelitemerSymbol = '=';

    readonly string commandName;
    readonly Dictionary<string, string> commandOptions;

    public string Name
    {
        get
        {
            return commandName;
        }
    }

    /// <summary>
    /// Cannot sent values from here
    /// </summary>
    public Dictionary<string, string> Options
    {
        get
        {
            return commandOptions.ToDictionary(co => co.Key, co => co.Value);
        }
    }

    public static char[] ForbiddenSymbolsInCommandName
    {
        get
        {
            return new char[] { OptionsDelitemerSymbol, OptionsBeginDelitemerSymbol, OptionValueDelitemerSymbol };
        }
    }

    public NetworkCommandData(string commandName, Dictionary<string, string> commandOptions)
    {
        if (string.IsNullOrEmpty(commandName) || commandName.Length < MinCommandNameLength)
        {
            throw new ArgumentException("Command Name must be at least " + MinCommandNameLength + " symbols long", "commandName");
        }

        if (HaveCommandNameForbiddenSymbols(commandName))
        {
            var forbiddenSymbols = ForbiddenSymbolsInCommandName.Select(s => s.ToString()).ToArray();
            throw new ArgumentException("Command name cannot contain " + string.Join(" ", forbiddenSymbols));
        }

        if (commandOptions == null)
        {
            throw new ArgumentNullException("commandOptions");
        }

        foreach (var option in commandOptions.ToList())
        {
            ValidateOption(option.Key, option.Value);
        }

        this.commandName = commandName;
        this.commandOptions = commandOptions;
    }

    public NetworkCommandData(string commandName)
        : this(commandName, new Dictionary<string, string>())
    {
    }

    public void AddOption(string optionName, string optionValue)
    {
        ValidateOption(optionName, optionValue);

        if (commandOptions.ContainsKey(optionName))
        {
            throw new ArgumentException("Already have option with name " + optionName);
        }

        commandOptions.Add(optionName, optionValue);
    }

    public void RemoveOption(string optionName)
    {
        if (!commandOptions.ContainsKey(optionName))
        {
            throw new ArgumentException("Cant find option with name " + optionName);
        }

        commandOptions.Remove(optionName);
    }

    public static NetworkCommandData Parse(string command)
    {
        if (!command.Contains(CommandTag))
        {
            throw new ArgumentException("Invalid command");
        }

        if (command.Length < (CommandTag.Length + MinCommandNameLength))
        {
            throw new ArgumentException("Command must be at least " + MinCommandNameLength + " symbols long");
        }

        var commandWithoutTag = command.Substring(CommandTag.Length);
        var optionsBeginDelitemerIndex = commandWithoutTag.IndexOf(OptionsBeginDelitemerSymbol);

        if (optionsBeginDelitemerIndex < 0)
        {
            return new NetworkCommandData(commandWithoutTag);
        }

        var commandName = commandWithoutTag.Remove(optionsBeginDelitemerIndex);

        if (commandName.Length < MinCommandNameLength)
        {
            throw new ArgumentException("Command name must be min " + MinCommandNameLength + " symbols long");
        }

        var commandOptionsValues = new Dictionary<string, string>();
        var options = commandWithoutTag.Substring(optionsBeginDelitemerIndex + 1)
            .Split(new char[] { OptionsDelitemerSymbol }, StringSplitOptions.RemoveEmptyEntries);

        if (options.Length < 1)
        {
            throw new ArgumentException("Invalid command");
        }

        for (int i = 0; i < options.Length; i++)
        {
            var option = options[i];
            var optionNameValueDelitemerIndex = option.IndexOf(OptionValueDelitemerSymbol);

            if (optionNameValueDelitemerIndex < 0)
            {
                throw new ArgumentException("Option must have value");
            }

            var optionNameValue = option.Split(new char[] { OptionValueDelitemerSymbol }, StringSplitOptions.RemoveEmptyEntries);

            if (optionNameValue.Length != 2)
            {
                throw new ArgumentException("Invalid option - " + option);
            }

            var optionName = optionNameValue[0];
            var optionValue = optionNameValue[1];

            if (commandOptionsValues.ContainsKey(optionName))
            {
                throw new ArgumentException("Cannot have options with same names");
            }

            ValidateOption(optionName, optionValue);
            commandOptionsValues.Add(optionName, optionValue);
        }

        return new NetworkCommandData(commandName, commandOptionsValues);
    }

    static void ValidateOption(string optionName, string optionValue)
    {
        if (string.IsNullOrEmpty(optionName) || optionName.Length < MinOptionNameLength)
        {
            throw new ArgumentException("Option name must be at least " + MinOptionNameLength + " symbols long");
        } 

        if (string.IsNullOrEmpty(optionValue) || optionValue.Length < MinOptionValueLength)
        {
            throw new ArgumentException("Option " + optionName + " value must be at least " + MinOptionValueLength + " symbols long");
        }
    }

    bool HaveCommandNameForbiddenSymbols(string commandName)
    {
        return ForbiddenSymbolsInCommandName.Any(c => commandName.Contains(c));
    }

    public override string ToString()
    {
        var command = new StringBuilder();

        command.Append(CommandTag)
            .Append(Name);

        if (Options.Count > 0)
        {
            command.Append(OptionsBeginDelitemerSymbol);
        }
        
        foreach (var parameter in Options.ToList())
        {
            var parameterName = parameter.Key;
            var parameterValue = parameter.Value;

            if (string.IsNullOrEmpty(parameterName) ||
                parameterName.Length < MinOptionNameLength ||
                string.IsNullOrEmpty(parameterValue) ||
                parameterValue.Length < MinOptionValueLength)
            {
                continue;
            }

            command.Append(parameterName)
                .Append(OptionValueDelitemerSymbol)
                .Append(parameterValue)
                .Append(OptionsDelitemerSymbol);
        }

        return command.ToString();
    }
}
