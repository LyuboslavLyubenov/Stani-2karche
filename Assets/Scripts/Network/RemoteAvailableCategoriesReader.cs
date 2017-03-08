namespace Assets.Scripts.Network
{
    using System;
    using System.Timers;

    using Assets.Scripts.Interfaces.Network.NetworkManager;

    using Commands;
    using Commands.Client;
    using Interfaces;
    using NetworkManagers;
    using Utils;

    public class RemoteAvailableCategoriesReader : IAvailableCategoriesReader, IDisposable
    {
        private IClientNetworkManager networkManager;

        private Action onTimeout;

        private Timer_ExecuteMethodAfterTime timeoutTimer = null;

        public bool Receiving
        {
            get;
            private set;
        }

        private int timeoutInSeconds;

        public RemoteAvailableCategoriesReader(IClientNetworkManager networkManager, Action onTimeout, int timeoutInSeconds)
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

        private void OnTimeout()
        {
            ThreadUtils.Instance.RunOnMainThread(() =>
                {
                    if (!this.Receiving)
                    {
                        return;
                    }

                    try
                    {
                        this.StopReceivingAvailableCategories();
                    }
                    finally
                    {
                        this.onTimeout();
                    }
                });
        }

        private void StopReceivingAvailableCategories()
        {
            this.Receiving = false;
            this.networkManager.CommandsManager.RemoveCommand("AvailableCategories");        
        }

        public void Dispose()
        {
            try
            {
                this.timeoutTimer.Stop();

            }
            finally
            {
                this.timeoutTimer.Dispose();
                this.timeoutTimer = null;
                this.StopReceivingAvailableCategories();
            }
        }

        public void GetAllCategories(Action<string[]> onGetAllCategories)
        {
            if (onGetAllCategories == null)
            {
                throw new ArgumentNullException("onGetAllCategories");
            }

            var commandData = new NetworkCommandData("GetAvailableCategories");
            var receivedAvailableCategoriesCommand = new AvailableCategoriesCommand(onGetAllCategories);

            receivedAvailableCategoriesCommand.OnFinishedExecution += (sender, args) =>
                {
                    this.Receiving = false;
                };

            this.networkManager.SendServerCommand(commandData);
            this.networkManager.CommandsManager.AddCommand("AvailableCategories", receivedAvailableCategoriesCommand);

            this.Receiving = true;

            this.timeoutTimer = TimerUtils.ExecuteAfter(this.timeoutInSeconds, this.OnTimeout);
            this.timeoutTimer.RunOnUnityThread = true;
            this.timeoutTimer.AutoDispose = false;
        }
    }

}