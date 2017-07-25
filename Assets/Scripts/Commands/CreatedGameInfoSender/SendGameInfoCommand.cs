using IGameServer = Interfaces.IGameServer;
using INetworkManagerCommand = Interfaces.Network.NetworkManager.INetworkManagerCommand;
using IServerNetworkManager = Interfaces.Network.NetworkManager.IServerNetworkManager;
using NetworkCommandData = Commands.NetworkCommandData;
using ThreadUtils = Utils.ThreadUtils;

namespace Assets.Scripts.Commands.CreatedGameInfoSender
{

    using System;
    using System.Collections;
    using System.Collections.Generic;

    using Assets.Scripts.Commands.CreatedGameInfoReceiver;
    using Assets.Scripts.Extensions;

    using UnityEngine;
    using UnityEngine.Networking;

    public class SendGameInfoCommand : INetworkManagerCommand
    {
        private readonly IServerNetworkManager networkManager;

        private readonly IGameServer gameServer;

        public SendGameInfoCommand(IServerNetworkManager networkManager, IGameServer gameServer)
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
            this.gameServer = gameServer;
        }
        
        public void Execute(Dictionary<string, string> commandsOptionsValues)
        {
            var connectionId = commandsOptionsValues["ConnectionId"].ConvertTo<int>();
            var gameInfo = GameInfoFactory.Instance.Get(this.networkManager, this.gameServer);
            var gameInfoJSON = JsonUtility.ToJson(gameInfo);
            var receivedGameInfoCommand = NetworkCommandData.From<ReceivedGameInfoCommand>();
            receivedGameInfoCommand.AddOption("GameInfoJSON", gameInfoJSON);
            this.networkManager.SendClientCommand(connectionId, receivedGameInfoCommand);
        }
    }
}