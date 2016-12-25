using System;
using System.Collections.Generic;

namespace Assets.Scripts.Commands.Jokers
{

    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Jokers;
    using Assets.Scripts.Network;

    using EventArgs = System.EventArgs;

    public class SelectedDisableRandomAnswersJokerCommand : INetworkManagerCommand, INetworkOperationExecutedCallback
    {
        public EventHandler OnExecuted
        {
            get;
            set;
        }

        MainPlayerData mainPlayerData;
        DisableRandomAnswersJokerRouter jokerRouter;
        ServerNetworkManager networkManager;
        int answersToDisableCount;
        Type jokerType;

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