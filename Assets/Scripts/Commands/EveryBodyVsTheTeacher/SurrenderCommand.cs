using GameEndCommand = Commands.Client.GameEndCommand;
using IEveryBodyVsTheTeacherServer = Interfaces.Network.IEveryBodyVsTheTeacherServer;
using IGameDataIterator = Interfaces.GameData.IGameDataIterator;
using INetworkManagerCommand = Interfaces.Network.NetworkManager.INetworkManagerCommand;
using IServerNetworkManager = Interfaces.Network.NetworkManager.IServerNetworkManager;
using NetworkCommandData = Commands.NetworkCommandData;
using ThreadUtils = Utils.ThreadUtils;

namespace Assets.Scripts.Commands.EveryBodyVsTheTeacher
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using Assets.Scripts.Extensions;

    using UnityEngine;

    public class SurrenderCommand : INetworkManagerCommand
    {
        private readonly IServerNetworkManager networkManager;
        private readonly IEveryBodyVsTheTeacherServer server;
        private readonly IGameDataIterator iterator;

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

        private IEnumerator SurrenderCoroutine(int connectionId)
        {
            var gameEndCommand = NetworkCommandData.From<GameEndCommand>();
            gameEndCommand.AddOption("Mark", this.iterator.CurrentMark.ToString());
            this.networkManager.SendClientCommand(connectionId, gameEndCommand);

            yield return new WaitForSeconds(1f);
            
            this.networkManager.KickPlayer(connectionId, "Успешно се отказа за игра.");
            this.networkManager.BanPlayer(connectionId);

            var areAllMainPlayersBanned =
                !this.server.MainPlayersConnectionIds
                    .Except(this.networkManager.BannedClientsConnectionIds)
                    .Any();

            if (areAllMainPlayersBanned)
            {
                this.server.EndGame();
            }
        }

        public void Execute(Dictionary<string, string> commandsOptionsValues)
        {
            var connectionId = commandsOptionsValues["ConnectionId"].ConvertTo<int>();

            var isMainPlayer = this.server.ConnectedMainPlayersConnectionIds.Contains(connectionId);
            if (!isMainPlayer || this.server.IsGameOver || !this.server.StartedGame)
            {
                return;
            }

            ThreadUtils.Instance.RunOnMainThread(this.SurrenderCoroutine(connectionId));
        }
    }
}