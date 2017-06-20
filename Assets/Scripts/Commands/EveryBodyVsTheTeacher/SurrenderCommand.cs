using GameEndCommand = Commands.Client.GameEndCommand;
using IEveryBodyVsTheTeacherServer = Interfaces.Network.IEveryBodyVsTheTeacherServer;
using IGameDataIterator = Interfaces.GameData.IGameDataIterator;
using INetworkManagerCommand = Interfaces.Network.NetworkManager.INetworkManagerCommand;
using INetworkOperationExecutedCallback = Interfaces.Network.NetworkManager.INetworkOperationExecutedCallback;
using IServerNetworkManager = Interfaces.Network.NetworkManager.IServerNetworkManager;
using NetworkCommandData = Commands.NetworkCommandData;

namespace Assets.Scripts.Commands.EveryBodyVsTheTeacher
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Assets.Scripts.Extensions;

    public class SurrenderCommand : INetworkManagerCommand, INetworkOperationExecutedCallback
    {
        private readonly IServerNetworkManager networkManager;
        private readonly IEveryBodyVsTheTeacherServer server;
        private readonly IGameDataIterator iterator;

        public EventHandler OnExecuted
        {
            get; set;
        }

        public SurrenderCommand(
            IServerNetworkManager networkManager, 
            IEveryBodyVsTheTeacherServer server, 
            IGameDataIterator iterator)
        {
            if (networkManager == null)
            {
                throw new ArgumentNullException("networkManager");
            }

            if (server == null)
            {
                throw new ArgumentNullException("server");
            }

            if (iterator == null)
            {
                throw new ArgumentNullException("iterator");
            }
            
            this.networkManager = networkManager;
            this.server = server;
            this.iterator = iterator;
        }
        
        public void Execute(Dictionary<string, string> commandsOptionsValues)
        {
            var connectionId = commandsOptionsValues["ConnectionId"].ConvertTo<int>();
            var isMainPlayer = this.server.ConnectedMainPlayersConnectionIds.Contains(connectionId);
            var isMainPlayerAlreadySurrendered = this.server.SurrenderedMainPlayersConnectionIds.Contains(connectionId);

            if (!isMainPlayer || 
                this.server.IsGameOver || 
                !this.server.StartedGame || 
                isMainPlayerAlreadySurrendered)
            {
                return;
            }

            var gameEndCommand = NetworkCommandData.From<GameEndCommand>();
            gameEndCommand.AddOption("Mark", this.iterator.CurrentMark.ToString());
            this.networkManager.SendClientCommand(connectionId, gameEndCommand);

            this.server.AddMainPlayerToSurrenderList(connectionId);

            var areAllMainPlayersSurrenderedGame = 
                !this.server.SurrenderedMainPlayersConnectionIds
                    .Except(this.server.MainPlayersConnectionIds)
                    .Any();
            if (areAllMainPlayersSurrenderedGame)
            {
                this.server.EndGame();
            }
        }
    }
}