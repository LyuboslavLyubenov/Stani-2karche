namespace Assets.Scripts.Commands.Jokers
{
    using System;
    using System.Collections.Generic;

    using Assets.Scripts.Interfaces.Network.Jokers;
    using Assets.Scripts.Interfaces.Network.Jokers.Routers;
    using Assets.Scripts.Interfaces.Network.NetworkManager;

    using DTOs;
    using Interfaces;
    using Scripts.Jokers;

    using EventArgs = System.EventArgs;

    public class SelectedHelpFromAudienceJokerRouterCommand : INetworkManagerCommand, INetworkOperationExecutedCallback
    {
        public EventHandler OnExecuted
        {
            get;
            set;
        }

        private MainPlayerData mainPlayerData;

        private IHelpFromAudienceJokerRouter askAudienceJokerRouter;
        
        private Type askAudienceJokerType;

        private int timeToAnswerInSeconds;

        public SelectedHelpFromAudienceJokerRouterCommand(
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