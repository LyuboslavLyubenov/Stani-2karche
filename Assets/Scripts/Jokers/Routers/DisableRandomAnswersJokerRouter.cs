namespace Jokers.Routers
{
    using System;

    using Commands;
    
    using Interfaces.Network.Jokers.Routers;
    using Interfaces.Network.NetworkManager;

    using EventArgs = System.EventArgs;

    public class DisableRandomAnswersJokerRouter : IDisableRandomAnswersRouter
    {
        private readonly IServerNetworkManager networkManager;
        private readonly int connectioId;

        public event EventHandler OnActivated = delegate
            {
            };

        public event EventHandler<UnhandledExceptionEventArgs> OnError = delegate
            {
            };

        public DisableRandomAnswersJokerRouter(IServerNetworkManager networkManager, int connectioId)
        {
            if (networkManager == null)
            {
                throw new ArgumentNullException("networkManager");
            }

            if (connectioId <= 0)
            {
                throw new ArgumentOutOfRangeException("connectioId");
            }
            
            this.networkManager = networkManager;
            this.connectioId = connectioId;
        }

        public void Activate(int answersToDisableCount)
        {
            if (answersToDisableCount < 0)
            {
                throw new ArgumentOutOfRangeException("answersToDisableCount");
            }

            var settingsCommand = new NetworkCommandData("DisableRandomAnswersJokerSettings");
            settingsCommand.AddOption("AnswersToDisableCount", answersToDisableCount.ToString());
            this.networkManager.SendClientCommand(this.connectioId, settingsCommand);

            this.OnActivated(this, EventArgs.Empty);
        }
    }
}