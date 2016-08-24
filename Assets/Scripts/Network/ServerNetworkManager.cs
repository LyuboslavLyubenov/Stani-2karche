using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public class ServerNetworkManager : ExtendedMonoBehaviour
{
    const int Port = 7788;
    const float CheckForDeadClientsDelayInSeconds = 3f;

    //how many clients can be connected to the server
    public int MaxConnections;
    public NotificationsServiceController NotificationServiceController;
    public LANServerOnlineBroadcastService LANServerOnlineBroadcastService;

    public EventHandler OnConnectedEvent = delegate
    {
    };

    public EventHandler<DataSentEventArgs> OnReceivedDataEvent = delegate
    {
    };

    public EventHandler OnDisconnectedEvent = delegate
    {
    };

    int genericHostId = 0;

    ConnectionConfig connectionConfig = null;
    HostTopology topology = null;

    byte communicationChannel = 0;

    bool isRunning = false;
    //Id of all connected clients
    List<int> connectedClientsIds = new List<int>();
    List<int> bannedConnections = new List<int>();

    HashSet<int> aliveClientsId = new HashSet<int>();
    //Their names
    Dictionary<int, string> connectedClientsNames = new Dictionary<int, string>();
    Dictionary<string, Action<NetworkData>> commandsFromClient = new Dictionary<string, Action<NetworkData>>();

    public bool IsRunning
    {
        get
        {
            return isRunning;
        }
    }

    public int[] BannedConnectionsIds
    {
        get
        {
            return bannedConnections.ToArray();
        }
    }

    public int ConnectedClientsCount
    {
        get
        {
            return connectedClientsIds.Count;
        }
    }

    public int[] ConnectedClientsIds
    {
        get
        {
            return aliveClientsId.ToArray();
        }
    }

    public Dictionary<int, string> ConnectedClientsIdsNames
    {
        get
        {
            return connectedClientsNames.ToDictionary(idUsername => idUsername.Key, idUsername => idUsername.Value);
        }
    }

    public string[] GetAllClientsNames()
    {
        return connectedClientsNames.Select(c => c.Value).ToArray();
    }

    void Start()
    {
        ConfigureCommands();
        ConfigureServer();
        StartServer();

        if (LANServerOnlineBroadcastService != null)
        {
            LANServerOnlineBroadcastService.Start();     
        }
       
        CoroutineUtils.RepeatEverySeconds(CheckForDeadClientsDelayInSeconds, UpdateAliveClients);
        StartCoroutine(UpdateCoroutine());
    }

    void OnDisable()
    {
        StopAllCoroutines();
        StopServer();
    }

    void ConfigureServer()
    {
        connectionConfig = new ConnectionConfig();
        communicationChannel = connectionConfig.AddChannel(QosType.ReliableSequenced);//make sure all messages are in order and received
        topology = new HostTopology(connectionConfig, MaxConnections);
    }

    void ConfigureCommands()
    {
        commandsFromClient["SetUsername"] = SetUsernameCommand;
        commandsFromClient["KeepAlive"] = ClientKeepAliveCommand;
    }

    void ShowNotification(Color color, string message)
    {
        if (NotificationServiceController != null)
        {
            NotificationServiceController.AddNotification(color, message);          
        }
    }

    IEnumerator UpdateCoroutine()
    {
        while (true)
        {
            if (isRunning)
            {
                UpdateServer();
            }

            yield return null;
        }
    }

    void UpdateServer()
    {
        NetworkData recieveNetworkData = null;
    
        try
        {
            recieveNetworkData = NetworkTransportUtils.ReceiveMessage();
        }
        catch (NetworkException e)
        {
            var error = (NetworkError)e.ErrorN;
            var errorMessage = NetworkErrorUtils.GetMessage(error);

            ShowNotification(Color.red, errorMessage);
            return;
        }

        switch (recieveNetworkData.NetworkEventType)
        {
            case NetworkEventType.ConnectEvent:
                OnClientConnected(recieveNetworkData);
                break;

            case NetworkEventType.DataEvent:
                OnClientSendData(recieveNetworkData);
                break;

            case NetworkEventType.DisconnectEvent:
                OnClientDisconnect(recieveNetworkData);
                break;
        }    

    }

    void UpdateAliveClients()
    {
        if (!isRunning)
        {
            return;
        }

        var aliveClientsConnectionIds = aliveClientsId.ToList();
        var deadClients = connectedClientsIds.Except(aliveClientsId);

        foreach (var deadClientConnectionId in deadClients)
        {
            try
            {
                byte error;
                NetworkTransport.Disconnect(genericHostId, deadClientConnectionId, out error);    
            }
            catch (Exception e)
            {
                Debug.Log("UpdateALiveClients - " + e.Message);
            }

            connectedClientsNames.Remove(deadClientConnectionId);
        }

        connectedClientsIds = aliveClientsConnectionIds;
        aliveClientsId.Clear();
    }

    void OnClientConnected(NetworkData networkData)
    {
        var connectionId = networkData.ConnectionId;
        connectedClientsIds.Add(connectionId);

        var isBanned = bannedConnections.IndexOf(connectionId) > -1;

        if (isBanned)
        {
            KickPlayer(connectionId, "Забранено ти е да влизаш във този сървър.");
            return;
        }

        SendClientMessage(connectionId, "AllowedToConnect");

        aliveClientsId.Add(connectionId);

        if (OnConnectedEvent != null)
        {
            OnConnectedEvent(this, EventArgs.Empty);    
        }
    }

    void OnClientDisconnect(NetworkData networkData)
    {
        //if disconnected remove from connected clients list
        var connectionId = networkData.ConnectionId;

        connectedClientsIds.Remove(connectionId);
        connectedClientsNames.Remove(connectionId);

        if (OnDisconnectedEvent != null)
        {
            OnDisconnectedEvent(this, EventArgs.Empty);    
        }
    }

    void OnClientSendData(NetworkData receiveNetworkData)
    {
        //if we received data from client
        var connectionId = receiveNetworkData.ConnectionId;
        var message = receiveNetworkData.Message;
        var commandAndParams = message.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
        var commandName = commandAndParams[0];
        var commandParams = commandAndParams.Skip(1).ToArray();
        var isValidCommand = (!string.IsNullOrEmpty(commandName) && commandsFromClient.ContainsKey(commandName));

        if (isValidCommand)
        {
            var commandToExecute = commandsFromClient[commandName];
            commandToExecute.Invoke(receiveNetworkData);
        }
        else
        {
            var username = GetClientUsername(connectionId); 

            if (OnReceivedDataEvent != null)
            {
                OnReceivedDataEvent(this, new DataSentEventArgs(receiveNetworkData.ConnectionId, username, message));    
            }
        }
    }

    string GetClientUsername(int connectionId)
    {
        var username = connectedClientsNames.FirstOrDefault(ci => ci.Key == connectionId);

        if (username.Equals(new KeyValuePair<int, string>()))
        {
            return "Клиент номер " + connectionId;
        }
        else
        {
            return username.Value;    
        }
    }

    void SetUsernameCommand(NetworkData networkData)
    {
        var commandParams = networkData.Message.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries).Skip(1).ToArray();
        var connectionId = networkData.ConnectionId;

        if (commandParams.Length < 1 || string.IsNullOrEmpty(commandParams[0]))
        {
            //empty username :(
            connectedClientsNames[connectionId] = "Играч Номер " + connectionId;
            return;
        }

        var username = commandParams[0];

        if (username.Length >= 4)
        {
            connectedClientsNames[connectionId] = username;
        }
        else
        {
            connectedClientsNames[connectionId] = "Играч Номер " + connectionId;
        }
    }

    void ClientKeepAliveCommand(NetworkData networkData)
    {
        aliveClientsId.Add(networkData.ConnectionId);
    }

    bool IsClientConnected(int connectionId)
    {
        return connectedClientsIds.IndexOf(connectionId) > -1;
    }

    public void StartServer()
    {
        NetworkTransport.Init();
        genericHostId = NetworkTransport.AddHost(topology, Port, null);
        isRunning = true;
    }

    public void StopServer()
    {
        NetworkTransport.RemoveHost(genericHostId);
        NetworkTransport.Shutdown();
        isRunning = false;
    }

    public void SendClientMessage(int clientId, string message)
    {
        try
        {
            NetworkTransportUtils.SendMessage(genericHostId, clientId, communicationChannel, message);
        }
        catch (NetworkException ex)
        {
            var errorN = ex.ErrorN;
            var error = (NetworkError)errorN;
            var errorMessage = NetworkErrorUtils.GetMessage(error);

            NotificationServiceController.AddNotification(Color.red, errorMessage);
        }
    }

    public void SendAllClientsMessage(string message)
    {
        for (int i = 0; i < connectedClientsIds.Count; i++)
        {
            var clientId = connectedClientsIds[i];
            SendClientMessage(clientId, message);
        }
    }

    public void KickPlayer(int connectionId, string message)
    {
        if (!IsClientConnected(connectionId))
        {
            return;    
        }   

        try
        {
            SendClientMessage(connectionId, "KickReason=" + message);    
        }
        catch (NetworkException ex)
        {
            Debug.Log(ex.Message);
        }

        CoroutineUtils.WaitForFrames(1, () =>
            {
                byte error;
                NetworkTransport.Disconnect(genericHostId, connectionId, out error);
            });
    }

    public void KickPlayer(int connectionId)
    {
        KickPlayer(connectionId, "Изгонен си от сървъра.");
    }

    public void BanPlayer(int connectionId)
    {
        if (bannedConnections.IndexOf(connectionId) > -1)
        {
            return;
        }

        bannedConnections.Add(connectionId);
        KickPlayer(connectionId, "Нямаш право да влизаш във сървъра.");
    }

    void OnGUI()
    {
        GUI.Box(new Rect(0, 0, 315, 300), "ServerNetworkManager debug");

        var connectedPlayersRect = new Rect(5, 30, 150, 30);
        var banRandomPlayerButtonRect = new Rect(5, 55, 145, 30);
        var kickRandomPlayerButtonRect = new Rect(160, 55, 145, 30);

        var banRandomClientButton = GUI.Button(banRandomPlayerButtonRect, "Ban Random Client");
        var kickRandomClientButton = GUI.Button(kickRandomPlayerButtonRect, "Kick Random Client");

        GUI.Label(connectedPlayersRect, "Connected players " + connectedClientsIds.Count + '/' + MaxConnections);

        if (banRandomClientButton)
        {
            var randomClientId = connectedClientsIds.GetRandomElement();
            BanPlayer(randomClientId);
        }

        if (kickRandomClientButton)
        {
            var randomClientId = connectedClientsIds.GetRandomElement();
            KickPlayer(randomClientId);
        }
    }
}