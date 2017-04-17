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

        public event EventHandler OnActivated = delegate
            {
            };

        public event EventHandler<UnhandledExceptionEventArgs> OnError = delegate
            {
            };

        public DisableRandomAnswersJokerRouter(IServerNetworkManager networkManager)
        {
            if (networkManager == null)
            {
                throw new ArgumentNullException("networkManager");
            }
            
            this.networkManager = networkManager;
        }

        public void Activate(int answersToDisableCount, int connectionId)
        {
            if (answersToDisableCount < 0)
            {
                throw new ArgumentOutOfRangeException("answersToDisableCount");
            }

            if (connectionId <= 0)
            {
                throw new ArgumentOutOfRangeException("connectionId");
            }

            var settingsCommand = new NetworkCommandData("DisableRandomAnswersJokerSettings");
            settingsCommand.AddOption("AnswersToDisableCount", answersToDisableCount.ToString());
            this.networkManager.SendClientCommand(connectionId, settingsCommand);

            this.OnActivated(this, EventArgs.Empty);
        }
    }
}