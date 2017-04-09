namespace Commands.Jokers.JokerElection
{

    using System;
    using System.Collections.Generic;

    using Extensions;

    using Interfaces.Network.NetworkManager;
    public class PlayerSelectedJokerCommand : INetworkManagerCommand
    {
        public delegate void OnPlayerSelectedJoker(int connectionId);

        private readonly OnPlayerSelectedJoker onPlayerSelectedJoker;

        public PlayerSelectedJokerCommand(OnPlayerSelectedJoker onPlayerSelectedJoker)
        {
            if (onPlayerSelectedJoker == null)
            {
                throw new ArgumentNullException("onPlayerSelectedJoker");
            }

            this.onPlayerSelectedJoker = onPlayerSelectedJoker;
        }

        public void Execute(Dictionary<string, string> commandsOptionsValues)
        {
            var connectionId = commandsOptionsValues["ConnectionId"].ConvertTo<int>();
            this.onPlayerSelectedJoker(connectionId);
        }
    }
}
