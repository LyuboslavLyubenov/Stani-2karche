using IEveryBodyVsTheTeacherServer = Interfaces.Network.IEveryBodyVsTheTeacherServer;
using IServerNetworkManager = Interfaces.Network.NetworkManager.IServerNetworkManager;
using Server_MainPlayerConnectingCommand = Commands.Server.MainPlayerConnectingCommand;

namespace Assets.Scripts.Commands.Server.EveryBodyVsTheTeacher
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Assets.Scripts.Extensions;

    public class MainPlayerConnectingCommand : Server_MainPlayerConnectingCommand
    {
        private readonly IServerNetworkManager networkManager;
        private readonly IEveryBodyVsTheTeacherServer server;

        public MainPlayerConnectingCommand(
            IServerNetworkManager networkManager,
            IEveryBodyVsTheTeacherServer server, 
            PlayerConnectingDelegate onPlayerConnecting)
            : base(onPlayerConnecting)
        {
            if (networkManager == null)
            {
                throw new ArgumentNullException("networkManager");
            }

            if (server == null)
            {
                throw new ArgumentNullException("server");
            }

            this.networkManager = networkManager;
            this.server = server;
        }

        public override void Execute(Dictionary<string, string> commandsOptionsValues)
        {
            var connectionId = commandsOptionsValues["ConnectionId"]
                .ConvertTo<int>();

            var isMainPlayerSurrendered = this.server.SurrenderedMainPlayersConnectionIds.Contains(connectionId);
            if (isMainPlayerSurrendered)
            {
                this.networkManager.KickPlayer(connectionId);
            }
            else
            {
                base.Execute(commandsOptionsValues);
            }
        }
    }
}