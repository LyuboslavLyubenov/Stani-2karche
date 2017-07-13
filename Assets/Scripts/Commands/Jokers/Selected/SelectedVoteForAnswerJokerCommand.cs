namespace Commands.Jokers.Selected
{

    using System.Collections.Generic;
    using System.Linq;

    using Assets.Scripts.Extensions;

    using Interfaces.Network;
    using Interfaces.Network.NetworkManager;

    public class SelectedVoteForAnswerJokerCommand : INetworkManagerCommand
    {
        private readonly IEveryBodyVsTheTeacherServer server;
        private readonly IServerNetworkManager networkManager;

        readonly IList<int> votedClients = new List<int>();

        public SelectedVoteForAnswerJokerCommand(IEveryBodyVsTheTeacherServer server, IServerNetworkManager networkManager)
        {
            this.server = server;
            this.networkManager = networkManager;
        }

        public void Execute(Dictionary<string, string> commandsOptionsValues)
        {
            if (!this.server.StartedGame)
            {
                return;
            }

            var connectionId = commandsOptionsValues["ConnectionId"].ConvertTo<int>();

            if (!this.server.ConnectedMainPlayersConnectionIds.Contains(connectionId) || this.votedClients.Contains(connectionId))
            {
                return; 
            }

            this.votedClients.Add(connectionId);

            var selectedVoteForAnswerJokerCommand = NetworkCommandData.From<SelectedVoteForAnswerJokerCommand>();
            selectedVoteForAnswerJokerCommand.AddOption("SenderConnectionId", connectionId.ToString());

            this.networkManager.SendClientCommand(this.server.PresenterId, selectedVoteForAnswerJokerCommand);
        }
    }
}