using AskAudienceJoker = Jokers.AskAudienceJoker;

namespace Commands.Jokers.Selected
{
    using System;
    using System.Collections.Generic;

    using Interfaces.Network.Jokers.Routers;
    using Interfaces.Network.NetworkManager;

    using Network;

    using EventArgs = System.EventArgs;

    public class SelectedHelpFromAudienceJokerCommand : INetworkManagerCommand, INetworkOperationExecutedCallback
    {
        private MainPlayerData mainPlayerData;

        private IHelpFromAudienceJokerRouter askAudienceJokerRouter;
        
        private Type askAudienceJokerType;

        private int timeToAnswerInSeconds;

        public EventHandler OnExecuted
        {
            get;
            set;
        }

        public SelectedHelpFromAudienceJokerCommand(
            MainPlayerData mainPlayerData, 
            IHelpFromAudienceJokerRouter askAudienceJokerRouter,
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
            this.askAudienceJokerRouter.Activate(senderConnectionId, this.timeToAnswerInSeconds);

            if (this.OnExecuted != null)
            {
                this.OnExecuted(this, EventArgs.Empty);
            }
        }
    }
}