using INetworkManagerCommand = Interfaces.Network.NetworkManager.INetworkManagerCommand;

namespace Assets.Scripts.Utils
{
    public class NetworkManagerCommandUtils
    {
        NetworkManagerCommandUtils()
        {    
        }

        public static string GetCommandName<TCommand>() where TCommand : INetworkManagerCommand
        {
            return typeof(TCommand).Name.Replace("Command", "");
        }
    }
}
