using System;

namespace Assets.Scripts.Jokers
{

    using Commands;
    using Interfaces;

    using JetBrains.Annotations;

    using Network.NetworkManagers;

    using EventArgs = System.EventArgs;

    public class DisableRandomAnswersJokerRouter
    {
        private readonly ServerNetworkManager networkManager;

        private readonly IPlayerData playerData;

        public EventHandler OnActivated = delegate
            {
            };

        public EventHandler<UnhandledExceptionEventArgs> OnError = delegate
            {
            };

        public DisableRandomAnswersJokerRouter(ServerNetworkManager networkManager, IPlayerData playerData)
        {
            if (networkManager == null)
            {
                throw new ArgumentNullException("networkManager");
            }

            if (playerData == null)
            {
                throw new ArgumentNullException("playerData");
            }
            
            this.networkManager = networkManager;
            this.playerData = playerData;
        }

        public void Activate(int answersToDisableCount)
        {
            if (answersToDisableCount < 0)
            {
                throw new ArgumentOutOfRangeException("answersToDisableCount");
            }

            var settingsCommand = new NetworkCommandData("DisableRandomAnswersJokerSettings");
            settingsCommand.AddOption("AnswersToDisableCount", answersToDisableCount.ToString());
            networkManager.SendClientCommand(playerData.ConnectionId, settingsCommand);

            this.OnActivated(this, EventArgs.Empty);
        }
    }
}