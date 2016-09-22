using System;

public class MainPlayerData : IPlayerData
{
    ServerNetworkManager networkManager;

    public JokersData JokersData
    {
        get;
        private set;
    }

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

        JokersData = new JokersData();
        this.networkManager = networkManager;

        networkManager.OnClientDisconnected += OnClientDisconnected;
        networkManager.CommandsManager.AddCommand("MainPlayerConnecting", new ReceivedMainPlayerConnectingCommand(OnMainPlayerConnecting));

        //lazy motherf*cker
        OnConnected = delegate
        {
        };

        OnDisconnected = delegate
        {
        };
    }

    void OnClientDisconnected(object sender, ClientConnectionDataEventArgs args)
    {
        if (ConnectionId == args.ConnectionId)
        {
            IsConnected = false;
            OnDisconnected(this, args);
        }
    }

    void OnMainPlayerConnecting(int connectionId)
    {
        if (IsConnected)
        {
            return;
        }

        ConnectionId = connectionId;
        IsConnected = true;

        OnConnected(this, new ClientConnectionDataEventArgs(connectionId));
    }
}