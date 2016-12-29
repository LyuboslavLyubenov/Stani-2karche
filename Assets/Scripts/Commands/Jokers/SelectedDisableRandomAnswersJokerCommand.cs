namespace Assets.Scripts.Commands.Jokers
{
    using System;
    using System.Collections.Generic;

    using DTOs;
    using Interfaces;
    using Scripts.Jokers;
    using Network.NetworkManagers;

    using EventArgs = System.EventArgs;

    public class SelectedDisableRandomAnswersJokerCommand : INetworkManagerCommand, INetworkOperationExecutedCallback
    {
        public EventHandler OnExecuted
        {
            get;
            set;
        }

        private MainPlayerData mainPlayerData;

        private DisableRandomAnswersJokerRouter jokerRouter;

        private ServerNetworkManager networkManager;

        private int answersToDisableCount;

        private Type jokerType;

        public SelectedDisableRandomAnswersJokerCommand(
            MainPlayerData mainPlayerData, 
            DisableRandomAnswersJokerRouter jokerRouter, 
            ServerNetworkManager networkManager,
            int answersToDisableCount
            )
        {
            if (mainPlayerData == null)
            {
                throw new ArgumentNullException("mainPlayerData");   
            }

            if (jokerRouter == null)
            {
                throw new ArgumentNullException("jokerRouter");
            }
            
            if (networkManager == null)
            {
                throw new ArgumentNullException("networkManager");
            }
            
            this.mainPlayerData = mainPlayerData;
            this.jokerRouter = jokerRouter;
            this.networkManager = networkManager;
            this.answersToDisableCount = answersToDisableCount;
            this.jokerType = typeof(DisableRandomAnswersJoker);
        }

        public void Execute(Dictionary<string, string> commandsOptionsValues)
        {
            var senderConnectionId = int.Parse(commandsOptionsValues["ConnectionId"]);

            if (!this.mainPlayerData.JokersData.AvailableJokers.Contains(this.jokerType))
            {
                return;
            }

            this.mainPlayerData.JokersData.RemoveJoker(this.jokerType);
            this.jokerRouter.Activate(this.answersToDisableCount, this.mainPlayerData, this.networkManager);

            if (this.OnExecuted != null)
            {
                this.OnExecuted(this, EventArgs.Empty);
            }
        }
    }

}