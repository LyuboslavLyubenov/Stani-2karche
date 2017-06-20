using IEveryBodyVsTheTeacherServer = Interfaces.Network.IEveryBodyVsTheTeacherServer;
using Server_MainPlayerConnectingCommand = Commands.Server.MainPlayerConnectingCommand;

namespace Assets.Scripts.Commands.Server.EveryBodyVsTheTeacher
{

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Assets.Scripts.Extensions;

    public class MainPlayerConnectingCommand : Server_MainPlayerConnectingCommand
    {
        private readonly IEveryBodyVsTheTeacherServer server;

        public MainPlayerConnectingCommand(
            IEveryBodyVsTheTeacherServer server, 
            PlayerConnectingDelegate onPlayerConnecting)
            : base(onPlayerConnecting)
        {
            if (server == null)
            {
                throw new ArgumentNullException("server");
            }

            this.server = server;
        }

        public override void Execute(Dictionary<string, string> commandsOptionsValues)
        {
            var connectionId = commandsOptionsValues["ConnectionId"]
                .ConvertTo<int>();

            var isMainPlayerSurrendered = this.server.SurrenderedMainPlayersConnectionIds.Contains(connectionId);
            if (!isMainPlayerSurrendered)
            {
                base.Execute(commandsOptionsValues);
            }
        }
    }
}
