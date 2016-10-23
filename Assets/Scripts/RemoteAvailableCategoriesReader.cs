using System;
using System.Timers;

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

        timeoutTimer.AutoReset = false;
        timeoutTimer.Interval = timeoutInSeconds * 1000;
        timeoutTimer.Elapsed += (sender, args) => OnTimeout();

        var threadUtils = ThreadUtils.Instance;
    }

    void OnTimeout()
    {
        ThreadUtils.Instance.RunOnMainThread(() =>
            {
                if (!Receiving)
                {
                    return;
                }

                Receiving = false;
                StopReceivingAvailableCategories();
                onTimeout();   
            });
    }

    void StopReceivingAvailableCategories()
    {
        try
        {
            networkManager.CommandsManager.RemoveCommand("AvailableCategories");
            Receiving = false;
        }
        catch
        {

        }
    }

    public void Dispose()
    {
        timeoutTimer.Dispose();
        StopReceivingAvailableCategories();
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
            Receiving = false;
        };

        networkManager.SendServerCommand(commandData);
        networkManager.CommandsManager.AddCommand("AvailableCategories", receivedAvailableCategoriesCommand);

        Receiving = true;

        timeoutTimer.Start();
    }
}