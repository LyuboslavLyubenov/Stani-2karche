namespace Assets.Scripts.Interfaces.Network.NetworkManager
{

    using Assets.Scripts.Commands;

    public interface ICommandsManager
    {
        void AddCommand(INetworkManagerCommand commandToExecute);

        void AddCommand(string commandName, INetworkManagerCommand commandToExecute);

        bool Exists<T>();

        bool Exists(string commandName);

        void RemoveCommand<T>();

        void RemoveCommand(string commandName);

        void RemoveAllCommands();

        void Execute(NetworkCommandData command);
    }

}