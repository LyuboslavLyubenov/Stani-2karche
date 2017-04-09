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
        private readonly int connectionId;

        public event EventHandler OnActivated = delegate
            {
            };

        public event EventHandler<UnhandledExceptionEventArgs> OnError = delegate
            {
            };

        public DisableRandomAnswersJokerRouter(IServerNetworkManager networkManager, int connectionId)
        {
            if (networkManager == null)
            {
                throw new ArgumentNullException("networkManager");
            }

            if (connectionId <= 0)
            {
                throw new ArgumentOutOfRangeException("connectionId");
            }
            
            this.networkManager = networkManager;
            this.connectionId = connectionId;
        }

        public void Activate(int answersToDisableCount)
        {
            if (answersToDisableCount < 0)
            {
                throw new ArgumentOutOfRangeException("answersToDisableCount");
            }

            var settingsCommand = new NetworkCommandData("DisableRandomAnswersJokerSettings");
            settingsCommand.AddOption("AnswersToDisableCount", answersToDisableCount.ToString());
            this.networkManager.SendClientCommand(this.connectionId, settingsCommand);

            this.OnActivated(this, EventArgs.Empty);
        }
    }
}