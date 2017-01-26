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

        private int answersToDisableCount;

        private Type jokerType;

        public SelectedDisableRandomAnswersJokerCommand(
            MainPlayerData mainPlayerData, 
            DisableRandomAnswersJokerRouter jokerRouter,
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
            
            
            this.mainPlayerData = mainPlayerData;
            this.jokerRouter = jokerRouter;
            this.answersToDisableCount = answersToDisableCount;
            this.jokerType = typeof(DisableRandomAnswersJoker);
        }

        public void Execute(Dictionary<string, string> commandsOptionsValues)
        {
            if (!this.mainPlayerData.JokersData.AvailableJokers.Contains(this.jokerType))
            {
                return;
            }

            this.mainPlayerData.JokersData.RemoveJoker(this.jokerType);
            this.jokerRouter.Activate(this.answersToDisableCount);

            if (this.OnExecuted != null)
            {
                this.OnExecuted(this, EventArgs.Empty);
            }
        }
    }

}