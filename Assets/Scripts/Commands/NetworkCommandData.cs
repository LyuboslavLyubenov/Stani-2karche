namespace Commands
{

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class NetworkCommandData
    {
        public const int CODE_Option_ClientConnectionId_All = -1;
        public const int CODE_Option_ClientConnectionId_Random = -2;
        public const int CODE_Option_ClientConnectionId_AI = -3;

        private const int MinCommandNameLength = 3;
        private const int MaxCommandNameLength = 100;

        private const int MinOptionsCount = 1;

        private const int MinOptionNameLength = 1;
        private const int MinOptionValueLength = 1;

        private const string NotEnoughSymbols = "{0} must be at least {1} symbols long";
        private const string TooMuchSymbols = "{0} must be less than {1} symbols long";
        private const string OptionAlreadyExists = "Already have option with name {0}";
        private const string CantFindOption = "Cant find option with name {0}";

        private readonly string commandName;
        private readonly Dictionary<string, string> commandOptions;

        public string Name
        {
            get
            {
                return this.commandName;
            }
        }

        /// <summary>
        /// Cannot set values from here
        /// </summary>
        public Dictionary<string, string> Options
        {
            get
            {
                return this.commandOptions.ToDictionary(co => co.Key, co => co.Value);
            }
        }

        public NetworkCommandData(string commandName, Dictionary<string, string> commandOptions)
        {
            ValidateCommandName(commandName);

            if (commandOptions == null)
            {
                throw new ArgumentNullException("commandOptions");
            }

            foreach (var option in commandOptions.ToList())
            {
                this.ValidateOption(option.Key, option.Value);
            }

            this.commandName = commandName;
            this.commandOptions = commandOptions;
        }

        public NetworkCommandData(string commandName)
            : this(commandName, new Dictionary<string, string>())
        {
        }

        public static NetworkCommandData From<T>()
        {
            var commandName = typeof(T).Name.Replace("Command", "");
            return new NetworkCommandData(commandName);
        }

        private static void ValidateCommandName(string commandName)
        {
            if (string.IsNullOrEmpty(commandName) || commandName.Trim().Length < MinCommandNameLength)
            {
                var exceptionMessage = string.Format(NotEnoughSymbols, "commandName", MinCommandNameLength);
                throw new ArgumentException(exceptionMessage);
            }

            if (commandName.Length > MaxCommandNameLength)
            {
                var exceptionMessage = string.Format(TooMuchSymbols, "commandName", MaxCommandNameLength);
                throw new ArgumentException(exceptionMessage);
            }
        }

        private void ValidateOption(string optionName, string optionValue)
        {
            if (string.IsNullOrEmpty(optionName) || optionName.Trim().Length < MinOptionNameLength)
            {
                var exceptionMessage = string.Format(NotEnoughSymbols, "optionName", MinOptionNameLength);
                throw new ArgumentException(exceptionMessage);
            } 

            if (string.IsNullOrEmpty(optionValue) || optionValue.Trim().Length < MinOptionValueLength)
            {
                var exceptionMessage = string.Format(NotEnoughSymbols, "optionValue", MinOptionValueLength);
                throw new ArgumentException(exceptionMessage);
            }
        }
       
        private static int FilterNameLength(string[] commandArgs)
        {
            int commandNameLength = -1;

            try
            {
                commandNameLength = int.Parse(commandArgs[0]);
            }
            catch (ArgumentNullException)
            {
                throw new ArgumentNullException();
            }
            catch
            {
                throw new ArgumentException("Invalid command name length");
            }

            if (commandNameLength < MinCommandNameLength)
            {
                var exceptionMessage = string.Format(NotEnoughSymbols, "commandName", MinCommandNameLength);
                throw new ArgumentException(exceptionMessage);
            }

            return commandNameLength;
        }

        private static int FilterOptionsCount(string[] commandArgs)
        {
            int optionsCount = -1;

            try
            {
                optionsCount = int.Parse(commandArgs[1]);
            }
            catch (ArgumentNullException)
            {
                throw new ArgumentNullException();
            }
            catch
            {
                throw new ArgumentException("Invalid command options count");
            }

            return optionsCount;
        }

        private static int ParseOptionNameLength(string nameLength)
        {
            int result = -1;

            try
            {
                result = int.Parse(nameLength);
            }
            catch
            {
                throw new ArgumentException("Invalid option length");
            }

            return result;
        }

        private static int ParseOptionValueLength(string valueLength)
        {
            int result;

            try
            {
                result = int.Parse(valueLength);
            }
            catch
            {
                throw new ArgumentException("Invalid value length");
            }

            return result;
        }


        private static Dictionary<int, int> FilterOptionNamesLengthValuesLength(string[] commandArgs, int optionsCount)
        {
            var optionsLengthValuesLength = new Dictionary<int, int>();

            for (int begin = 2, i = begin; (i < begin + (optionsCount * 2)) && (i < commandArgs.Length - 1); i += 2)
            {
                var option = ParseOptionNameLength(commandArgs[i]);
                var value = ParseOptionValueLength(commandArgs[i + 1]);

                if (option < MinOptionNameLength)
                {
                    var exceptionMessage = string.Format(NotEnoughSymbols, "optionName", MinOptionNameLength);
                    throw new ArgumentException(exceptionMessage);
                }

                if (value < MinOptionValueLength)
                {
                    var exceptionMessage = string.Format(NotEnoughSymbols, "optionValue", MinOptionValueLength);
                    throw new ArgumentException(exceptionMessage);
                }

                optionsLengthValuesLength.Add(option, value);
            }

            return optionsLengthValuesLength;
        }


        private static Dictionary<string, string> FilterOptionsValues(
            string text, 
            Dictionary<int, int> optionsNamesLengthValuesLength,
            int nameLength)
        {
            var result = new Dictionary<string, string>();
            var optionsText = text.Substring(nameLength);
            var filterIndex = 0;

            for (int i = 0; i < optionsNamesLengthValuesLength.Count; i++)
            {
                var namesLengthValuesLength = optionsNamesLengthValuesLength.Skip(i).First();
                var optionNameLength = namesLengthValuesLength.Key;
                var valueLength = namesLengthValuesLength.Value;
                var name = optionsText.Substring(filterIndex, optionNameLength);
                var value = optionsText.Substring(filterIndex + optionNameLength, valueLength);

                result.Add(name, value);

                filterIndex += optionNameLength + valueLength;
            }

            return result;
        }

        public void AddOption(string optionName, string optionValue)
        {
            this.ValidateOption(optionName, optionValue);

            if (this.commandOptions.ContainsKey(optionName))
            {
                var exceptionMessage = string.Format(OptionAlreadyExists, optionName);
                throw new InvalidOperationException(exceptionMessage);
            }

            this.commandOptions.Add(optionName, optionValue);
        }

        public void RemoveOption(string optionName)
        {
            if (!this.commandOptions.ContainsKey(optionName))
            {
                var excetionMessage = string.Format(CantFindOption, optionName);
                throw new InvalidOperationException(excetionMessage);
            }

            this.commandOptions.Remove(optionName);
        }

        public static NetworkCommandData Parse(string command)
        {
            if (string.IsNullOrEmpty(command) || command.Trim().Length < 1)
            {
                throw new ArgumentNullException();
            }

            var commandArgs = command.Split(' ');

            var nameLength = FilterNameLength(commandArgs);
            var optionsCount = FilterOptionsCount(commandArgs);
            var optionsNamesLengthValuesLength = FilterOptionNamesLengthValuesLength(commandArgs, optionsCount);

            if (optionsNamesLengthValuesLength.Count < optionsCount)
            {
                throw new ArgumentException("Invalid options length");
            }

            var textBeginIndex = 2 + (optionsCount * 2);
            var text = string.Join("", commandArgs.Skip(textBeginIndex).ToArray());

            if (text.Length < nameLength)
            {
                throw new ArgumentException("Name value length cant be less than stated length");
            }

            if (optionsCount == 0)
            {
                return new NetworkCommandData(text, new Dictionary<string, string>());
            }

            var name = text.Remove(nameLength);
            var commandOptionsValues = FilterOptionsValues(text, optionsNamesLengthValuesLength, nameLength);

            return new NetworkCommandData(name, commandOptionsValues);
        }


        public override string ToString()
        {
            var result = new StringBuilder();
            var allText = new StringBuilder();
            var nameLength = this.Name.Length;
            var optionsCount = this.Options.Count;
            var options = this.Options.ToList();

            allText.Append(this.Name);

            result.Append(nameLength)
                .Append(" ")
                .Append(optionsCount)
                .Append(" ");

            for (int i = 0; i < options.Count; i++)
            {
                var option = options[i];
                result.AppendFormat("{0} {1} ", option.Key.Length, option.Value.Length);

                allText.Append(option.Key)
                    .Append(option.Value);
            }

            result.Append(allText.ToString());

            return result.ToString();
        }
    }

}
