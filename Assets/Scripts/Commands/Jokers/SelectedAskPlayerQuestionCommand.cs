using System;
using System.Collections.Generic;

namespace Assets.Scripts.Commands.Jokers
{

    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Jokers;
    using Assets.Scripts.Jokers.AskPlayerQuestion;
    using Assets.Scripts.Network;

    using EventArgs = System.EventArgs;

    public class SelectedAskPlayerQuestionCommand : INetworkManagerCommand, INetworkOperationExecutedCallback
    {
        public EventHandler OnExecuted
        {
            get;
            set;
        }

        ServerNetworkManager networkManager;

        MainPlayerData mainPlayerData;
        AskPlayerQuestionRouter jokerServerRouter;
        int timeToAnswerInSeconds;

        Type helpFromFriendJokerType;

        public SelectedAskPlayerQuestionCommand(ServerNetworkManager networkManager, MainPlayerData mainPlayerData, AskPlayerQuestionRouter jokerServerRouter, int timeToAnswerInSeconds)
        {
            if (networkManager == null)
            {
                throw new ArgumentNullException("networkManager");
            }
            
            if (mainPlayerData == null)
            {
                throw new ArgumentNullException("mainPlayerData");
            }

            if (jokerServerRouter == null)
            {
                throw new ArgumentNullException("jokerServerRouter");
            }

            this.networkManager = networkManager;
            this.mainPlayerData = mainPlayerData;
            this.jokerServerRouter = jokerServerRouter;
            this.timeToAnswerInSeconds = timeToAnswerInSeconds;
            this.helpFromFriendJokerType = typeof(HelpFromFriendJoker);
        }

        public void Execute(Dictionary<string, string> commandsOptionsValues)
        {
            var senderConnectionId = int.Parse(commandsOptionsValues["ConnectionId"]);
            var sendClientId = int.Parse(commandsOptionsValues["PlayerConnectionId"]);

            if (!this.mainPlayerData.JokersData.AvailableJokers.Contains(this.helpFromFriendJokerType) || !this.networkManager.IsConnected(senderConnectionId))
            {
                return;
            }

            this.mainPlayerData.JokersData.RemoveJoker(this.helpFromFriendJokerType);
            this.jokerServerRouter.Activate(this.timeToAnswerInSeconds, senderConnectionId, sendClientId);

            if (this.OnExecuted != null)
            {
                this.OnExecuted(this, EventArgs.Empty);
            }
        }
    }

}
