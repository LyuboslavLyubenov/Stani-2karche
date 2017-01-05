using System;

namespace Assets.Scripts.Jokers
{

    using Commands;
    using Interfaces;
    using Network.NetworkManagers;

    using EventArgs = System.EventArgs;

    public class DisableRandomAnswersJokerRouter
    {
        public EventHandler OnActivated = delegate
            {
            };

        public EventHandler<UnhandledExceptionEventArgs> OnError = delegate
            {
            };

        public DisableRandomAnswersJokerRouter()
        {
            
        }

        public void Activate(int answersToDisableCount, IPlayerData playerData, ServerNetworkManager networkManager)
        {
            if (answersToDisableCount < 0)
            {
                throw new ArgumentOutOfRangeException("answersToDisableCount");
            }

            if (playerData == null)
            {
                throw new ArgumentNullException("playerData");
            }

            if (networkManager == null)
            {
                throw new ArgumentNullException("networkManager");
            }

            var settingsCommand = new NetworkCommandData("DisableRandomAnswersJokerSettings");
            settingsCommand.AddOption("AnswersToDisableCount", answersToDisableCount.ToString());
            networkManager.SendClientCommand(playerData.ConnectionId, settingsCommand);

            this.OnActivated(this, EventArgs.Empty);
        }
    }
}