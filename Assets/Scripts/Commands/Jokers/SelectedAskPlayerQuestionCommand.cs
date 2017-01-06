namespace Assets.Scripts.Commands.Jokers
{
    using System;
    using System.Collections.Generic;

    using DTOs;
    using Interfaces;
    using Scripts.Jokers;
    using Scripts.Jokers.AskPlayerQuestion;
    using Network.NetworkManagers;

    using EventArgs = System.EventArgs;

    public class SelectedAskPlayerQuestionCommand : INetworkManagerCommand, INetworkOperationExecutedCallback
    {
        public EventHandler OnExecuted
        {
            get;
            set;
        }

        private ServerNetworkManager networkManager;

        private MainPlayerData mainPlayerData;

        private AskPlayerQuestionRouter jokerServerRouter;

        private int timeToAnswerInSeconds;

        private Type helpFromFriendJokerType;

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
            //from
            var senderConnectionId = int.Parse(commandsOptionsValues["ConnectionId"]);
            //to
            var sendClientId = int.Parse(commandsOptionsValues["PlayerConnectionId"]);

            if (!this.mainPlayerData.JokersData.AvailableJokers.Contains(this.helpFromFriendJokerType) || !this.networkManager.IsConnected(senderConnectionId))
            {
                return;
            }

            this.mainPlayerData.JokersData.RemoveJoker(this.helpFromFriendJokerType);
            this.jokerServerRouter.Activate(senderConnectionId, sendClientId, this.timeToAnswerInSeconds);

            if (this.OnExecuted != null)
            {
                this.OnExecuted(this, EventArgs.Empty);
            }
        }
    }

}
