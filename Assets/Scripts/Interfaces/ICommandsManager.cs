namespace Assets.Scripts.Interfaces
{
    using Commands;

    public interface ICommandsManager
    {
        void AddCommand(INetworkManagerCommand commandToExecute);

        void AddCommand(string commandName, INetworkManagerCommand commandToExecute);

        void RemoveCommand<T>();

        void RemoveCommand(string commandName);

        void RemoveAllCommands();

        void Execute(NetworkCommandData command);
    }

}