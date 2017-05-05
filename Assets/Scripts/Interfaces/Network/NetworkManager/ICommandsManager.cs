using NetworkCommandData = Commands.NetworkCommandData;

namespace Interfaces.Network.NetworkManager
{

    using System.Collections.Generic;

    using Commands;

    public interface ICommandsManager
    {
        void AddCommand(INetworkManagerCommand commandToExecute);

        void AddCommand(string commandName, INetworkManagerCommand commandToExecute);

        void AddCommands(IEnumerable<INetworkManagerCommand> commands);

        bool Exists(INetworkManagerCommand command);

        bool Exists<T>();

        bool Exists(string commandName);

        void RemoveCommand<T>();

        void RemoveCommand(string commandName);

        void RemoveCommand(INetworkManagerCommand command);

        void RemoveAllCommands();

        void Execute(NetworkCommandData commandData);
    }

}