using System;

public class MainPlayerData : IPlayerData
{
    ServerNetworkManager networkManager;

    public EventHandler<ClientConnectionDataEventArgs> OnConnected
    {
        get;
        set;
    }

    public EventHandler<ClientConnectionDataEventArgs> OnDisconnected
    {
        get;
        set;
    }

    public JokersData JokersData
    {
        get;
        private set;
    }

    public bool IsConnected
    {
        get;
        private set;
    }

    public int ConnectionId
    {
        get;
        private set;
    }

    public string Username
    {
        get
        {
            if (!IsConnected)
            {
                throw new Exception("MainPlayer not connected to server");
            }

            return networkManager.GetClientUsername(ConnectionId);
        }
    }

    public MainPlayerData(ServerNetworkManager networkManager)
    {
        if (networkManager == null)
        {
            throw new ArgumentNullException("networkManager");
        }

        this.networkManager = networkManager;
        JokersData = new JokersData(networkManager);

        networkManager.OnClientDisconnected += OnClientDisconnected;
        networkManager.CommandsManager.AddCommand(new MainPlayerConnectingCommand(OnMainPlayerConnecting));

        //lazy motherf*cker
        OnConnected = delegate
        {
        };

        OnDisconnected = delegate
        {
        };

        IsConnected = false;
    }

    void OnClientDisconnected(object sender, ClientConnectionDataEventArgs args)
    {
        if (ConnectionId != args.ConnectionId)
        {
            return;
        }

        networkManager.CommandsManager.AddCommand(new MainPlayerConnectingCommand(OnMainPlayerConnecting));

        IsConnected = false;
        OnDisconnected(this, args);
    }

    void OnMainPlayerConnecting(int connectionId)
    {
        ConnectionId = connectionId;
        IsConnected = true;

        networkManager.CommandsManager.RemoveCommand<MainPlayerConnectingCommand>();

        OnConnected(this, new ClientConnectionDataEventArgs(connectionId));
    }
}