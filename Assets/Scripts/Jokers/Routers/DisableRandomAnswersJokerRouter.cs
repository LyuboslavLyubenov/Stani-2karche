namespace Assets.Scripts.Jokers.Routers
{

    using System;

    using Assets.Scripts.Commands;
    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Interfaces.Network.Jokers.Routers;
    using Assets.Scripts.Interfaces.Network.NetworkManager;

    using EventArgs = System.EventArgs;

    public class DisableRandomAnswersJokerRouter : IDisableRandomAnswersRouter
    {
        private readonly IServerNetworkManager networkManager;

        private readonly IPlayerData playerData;

        public event EventHandler OnActivated = delegate
            {
            };

        public event EventHandler<UnhandledExceptionEventArgs> OnError = delegate
            {
            };

        public DisableRandomAnswersJokerRouter(IServerNetworkManager networkManager, IPlayerData playerData)
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
            this.networkManager.SendClientCommand(this.playerData.ConnectionId, settingsCommand);

            this.OnActivated(this, EventArgs.Empty);
        }
    }
}