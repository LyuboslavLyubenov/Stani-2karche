using DisableRandomAnswersJoker = Jokers.DisableRandomAnswersJoker;
using MainPlayerData = Network.MainPlayerData;

namespace Commands.Jokers.Selected
{

    using System;
    using System.Collections.Generic;

    using Interfaces.Network.Jokers.Routers;
    using Interfaces.Network.NetworkManager;

    using EventArgs = System.EventArgs;

    public class SelectedDisableRandomAnswersJokerCommand : INetworkManagerCommand, INetworkOperationExecutedCallback
    {
        public EventHandler OnExecuted
        {
            get;
            set;
        }

        private MainPlayerData mainPlayerData;

        private IDisableRandomAnswersRouter jokerRouter;

        private readonly int connectionId;

        private int answersToDisableCount;

        private Type jokerType;

        public SelectedDisableRandomAnswersJokerCommand(
            MainPlayerData mainPlayerData,
            IDisableRandomAnswersRouter jokerRouter,
            int connectionId,
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

            if (connectionId <= 0)
            {
                throw new ArgumentOutOfRangeException("connectionId");
            }

            this.mainPlayerData = mainPlayerData;
            this.jokerRouter = jokerRouter;
            this.connectionId = connectionId;
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
            this.jokerRouter.Activate(this.answersToDisableCount, this.connectionId);

            if (this.OnExecuted != null)
            {
                this.OnExecuted(this, EventArgs.Empty);
            }
        }
    }

}