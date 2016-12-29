using System;
using System.Collections.Generic;

namespace Assets.Scripts.Commands.Jokers
{

    using Assets.Scripts.DTOs;
    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Jokers;
    using Assets.Scripts.Jokers.AudienceAnswerPoll;
    using Assets.Scripts.Network;
    using Assets.Scripts.Network.NetworkManagers;

    using EventArgs = System.EventArgs;

    public class SelectedAudiencePollCommand : INetworkManagerCommand, INetworkOperationExecutedCallback
    {
        public EventHandler OnExecuted
        {
            get;
            set;
        }

        MainPlayerData mainPlayerData;
        AudienceAnswerPollRouter askAudienceJokerRouter;
        ServerNetworkManager networkManager;
        Type askAudienceJokerType;
        int timeToAnswerInSeconds;

        public SelectedAudiencePollCommand(
            MainPlayerData mainPlayerData, 
            AudienceAnswerPollRouter askAudienceJokerRouter, 
            ServerNetworkManager networkManager, 
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

            if (networkManager == null)
            {
                throw new ArgumentNullException("networkManager");
            }
            
            this.mainPlayerData = mainPlayerData;
            this.askAudienceJokerRouter = askAudienceJokerRouter;
            this.networkManager = networkManager;
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
            this.askAudienceJokerRouter.Activate(senderConnectionId, this.mainPlayerData);

            if (this.OnExecuted != null)
            {
                this.OnExecuted(this, EventArgs.Empty);
            }
        }
    }

}