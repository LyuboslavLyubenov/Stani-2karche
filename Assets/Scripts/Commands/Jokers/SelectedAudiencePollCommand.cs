namespace Assets.Scripts.Commands.Jokers
{
    using System;
    using System.Collections.Generic;

    using Assets.Scripts.Interfaces.Network.Jokers;

    using DTOs;
    using Interfaces;
    using Scripts.Jokers;
    using Scripts.Jokers.AudienceAnswerPoll;

    using EventArgs = System.EventArgs;

    public class SelectedAudiencePollCommand : INetworkManagerCommand, INetworkOperationExecutedCallback
    {
        public EventHandler OnExecuted
        {
            get;
            set;
        }

        private MainPlayerData mainPlayerData;

        private IAudienceAnswerPollRouter askAudienceJokerRouter;
        

        private Type askAudienceJokerType;

        private int timeToAnswerInSeconds;

        public SelectedAudiencePollCommand(
            MainPlayerData mainPlayerData, 
            IAudienceAnswerPollRouter askAudienceJokerRouter,
            int timeToAnswerInSeconds
            )
        {
            if (mainPlayerData == null)
            {
                throw new ArgumentNullException("mainPlayerData");
            }

            if (askAudienceJokerRouter == null)
            {
                throw new ArgumentNullException("askAudienceJokerRouter");
            }
            
            this.mainPlayerData = mainPlayerData;
            this.askAudienceJokerRouter = askAudienceJokerRouter;
            this.timeToAnswerInSeconds = timeToAnswerInSeconds;
            this.askAudienceJokerType = typeof(AskAudienceJoker);
        }

        public void Execute(Dictionary<string, string> commandsOptionsValues)
        {
            var senderConnectionId = int.Parse(commandsOptionsValues["ConnectionId"]);

            if (!this.mainPlayerData.IsConnected ||
                this.mainPlayerData.ConnectionId != senderConnectionId ||
                !this.mainPlayerData.JokersData.AvailableJokers.Contains(this.askAudienceJokerType))
            {
                return;
            }

            this.mainPlayerData.JokersData.RemoveJoker(this.askAudienceJokerType);
            this.askAudienceJokerRouter.Activate(senderConnectionId, this.mainPlayerData, this.timeToAnswerInSeconds);

            if (this.OnExecuted != null)
            {
                this.OnExecuted(this, EventArgs.Empty);
            }
        }
    }

}