using ICreatedGameInfoSender = Interfaces.Network.NetworkManager.ICreatedGameInfoSender;
using IGameServer = Interfaces.IGameServer;
using IServerNetworkManager = Interfaces.Network.NetworkManager.IServerNetworkManager;

namespace Assets.Scripts.Network.GameInfo.New
{
    using System;

    using Assets.Scripts.Commands.CreatedGameInfoSender;

    public class CreatedGameInfoSender : ICreatedGameInfoSender
    {
        private readonly IServerNetworkManager networkManager;

        private readonly SendGameInfoCommand sendGameInfoCommand;

        public CreatedGameInfoSender(IServerNetworkManager networkManager, IGameServer gameServer)
        {
            if (networkManager == null)
            {
                throw new ArgumentNullException("networkManager");
            }

            if (gameServer == null)
            {
                throw new ArgumentNullException("gameServer");
            }

            this.networkManager = networkManager;
            this.sendGameInfoCommand = new SendGameInfoCommand(networkManager, gameServer);
            this.networkManager.CommandsManager.AddCommand(this.sendGameInfoCommand);
        }

        public void Dispose()
        {
            this.networkManager.CommandsManager.RemoveCommand(this.sendGameInfoCommand);
        }
    }
}