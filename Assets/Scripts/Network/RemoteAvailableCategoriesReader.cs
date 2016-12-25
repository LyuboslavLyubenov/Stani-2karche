using System;
using System.Timers;

namespace Assets.Scripts.Network
{

    using Assets.Scripts.Commands;
    using Assets.Scripts.Commands.Client;
    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Utils;

    public class RemoteAvailableCategoriesReader : IAvailableCategoriesReader, IDisposable
    {
        ClientNetworkManager networkManager;

        Action onTimeout;

        Timer timeoutTimer = new Timer();

        public bool Receiving
        {
            get;
            private set;
        }

        int timeoutInSeconds;

        public RemoteAvailableCategoriesReader(ClientNetworkManager networkManager, Action onTimeout, int timeoutInSeconds)
        {
            if (networkManager == null)
            {
                throw new ArgumentNullException("networkManager");
            }

            if (onTimeout == null)
            {
                throw new ArgumentNullException("onTimeout");
            }

            if (timeoutInSeconds <= 0)
            {
                throw new ArgumentOutOfRangeException("timeoutInSeconds");
            }
   
            this.networkManager = networkManager;
            this.onTimeout = onTimeout; 
            this.timeoutInSeconds = timeoutInSeconds;

            var threadUtils = ThreadUtils.Instance;
        }

        void OnTimeout()
        {
            ThreadUtils.Instance.RunOnMainThread(() =>
                {
                    if (!this.Receiving)
                    {
                        return;
                    }

                    this.StopReceivingAvailableCategories();
                    this.onTimeout();   
                });
        }

        void StopReceivingAvailableCategories()
        {
            try
            {
                this.Receiving = false;
                this.networkManager.CommandsManager.RemoveCommand("AvailableCategories");
            }
            catch
            {

            }
        }

        public void Dispose()
        {
            this.timeoutTimer.Dispose();
            this.StopReceivingAvailableCategories();
        }

        public void GetAllCategories(Action<string[]> onGetAllCategories)
        {
            if (onGetAllCategories == null)
            {
                throw new ArgumentNullException("onGetAllCategories");
            }

            var commandData = new NetworkCommandData("GetAvailableCategories");
            var receivedAvailableCategoriesCommand = new ReceivedAvailableCategoriesCommand(onGetAllCategories);

            receivedAvailableCategoriesCommand.OnFinishedExecution += (sender, args) =>
                {
                    this.Receiving = false;
                };

            this.networkManager.SendServerCommand(commandData);
            this.networkManager.CommandsManager.AddCommand("AvailableCategories", receivedAvailableCategoriesCommand);

            this.Receiving = true;

            this.timeoutTimer = new Timer();
            this.timeoutTimer.AutoReset = false;
            this.timeoutTimer.Interval = this.timeoutInSeconds * 1000;
            this.timeoutTimer.Elapsed += (sender, args) => this.OnTimeout();
        }
    }

}