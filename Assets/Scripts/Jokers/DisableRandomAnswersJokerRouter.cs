using System;

namespace Assets.Scripts.Jokers
{

    using Assets.Scripts.Commands;
    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Network;
    using Assets.Scripts.Utils;

    using EventArgs = System.EventArgs;

    public class DisableRandomAnswersJokerRouter : ExtendedMonoBehaviour
    {
        public EventHandler OnActivated = delegate
            {
            };

        public EventHandler<UnhandledExceptionEventArgs> OnError = delegate
            {
            };

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