using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using System.ComponentModel.Design;

public class ServerNetworkManager : ExtendedMonoBehaviour
{
    const int Port = 7788;
    const float CheckForDeadClientsDelayInSeconds = 6f;

    //how many clients can be connected to the server
    public int MaxConnections;
    public NotificationsServiceController NotificationServiceController;
    public LANServerOnlineBroadcastService LANServerOnlineBroadcastService;

    public EventHandler<ClientConnectionDataEventArgs> OnClientConnected = delegate
    {
    };

    public EventHandler<DataSentEventArgs> OnReceivedData = delegate
    {
    };

    public EventHandler<ClientConnectionDataEventArgs> OnClientDisconnected = delegate
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
    CommandsManager commandsManager = new CommandsManager();

    public bool IsRunning
    {
        get
        {
            return isRunning;
        }
    }

    public int ConnectedClientsCount
    {
        get
        {
            return connectedClientsIds.Count;
        }
    }

    public string[] GetAllClientsNames()
    {
        return connectedClientsNames.Select(c => c.Value).ToArray();
    }

    public CommandsManager CommandsManager
    {
        get
        {
            return commandsManager;
        }
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
        communicationChannel = connectionConfig.AddChannel(QosType.ReliableFragmented);
        topology = new HostTopology(connectionConfig, MaxConnections);
    }

    void ConfigureCommands()
    {
        commandsManager.AddCommand("SetUsername", new SetUsernameCommand(this));
        commandsManager.AddCommand("KeepAlive", new KeepAliveCommand(aliveClientsId));
        commandsManager.AddCommand("ConnectedClientsCount", new ServerSendConnectedClientsCountCommand(this));
        commandsManager.AddCommand("ConnectedClientsIdsNames", new ServerSendConnectedClientsIdsNamesCommand(this, connectedClientsNames));
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
        NetworkTransportUtils.ReceiveMessageAsync(ReceivedDataFromClientAsync, (exception) =>
            {
                var error = (NetworkError)exception.ErrorN;
                var errorMessage = NetworkErrorUtils.GetMessage(error);

                ShowNotification(Color.red, errorMessage);
            });
    }

    void ReceivedDataFromClientAsync(NetworkData networkData)
    {
        switch (networkData.NetworkEventType)
        {
            case NetworkEventType.ConnectEvent:
                OnConnectedClient(networkData);
                break;

            case NetworkEventType.DataEvent:
                OnClientSendData(networkData);
                break;

            case NetworkEventType.DisconnectEvent:
                OnClientDisconnect(networkData);
                break;
        }    
    }

    IList<int> GetDeadClientsIds(ICollection<int> aliveClientsIds)
    {
        var result = new List<int>();

        for (int i = 0; i < connectedClientsIds.Count; i++)
        {
            var clientId = connectedClientsIds[i];

            if (!aliveClientsId.Contains(clientId))
            {
                result.Add(clientId);
            }
        }

        return result;
    }

    void UpdateAliveClients()
    {
        if (!isRunning)
        {
            return;
        }

        var deadClientsIds = GetDeadClientsIds(aliveClientsId);

        for (int i = 0; i < deadClientsIds.Count; i++)
        {
            var deadClientConnectionId = deadClientsIds[i];
            //better safe than sorry
            try
            {
                byte error;
                NetworkTransport.Disconnect(genericHostId, deadClientConnectionId, out error);    
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }

            connectedClientsNames.Remove(deadClientConnectionId);
        }

        connectedClientsIds.RemoveAll(deadClientsIds.Contains);
        aliveClientsId.Clear();
    }

    void OnConnectedClient(NetworkData networkData)
    {
        var connectionId = networkData.ConnectionId;
        connectedClientsIds.Add(connectionId);

        var isBanned = bannedConnections.IndexOf(connectionId) > -1;

        if (isBanned)
        {
            KickPlayer(connectionId, "Забранено ти е да влизаш във този сървър.");
            return;
        }

        var commandData = new NetworkCommandData("AllowedToConnect");
        SendClientCommand(connectionId, commandData);

        aliveClientsId.Add(connectionId);

        if (OnClientConnected != null)
        {
            OnClientConnected(this, new ClientConnectionDataEventArgs(connectionId));    
        }
    }

    void OnClientDisconnect(NetworkData networkData)
    {
        //if disconnected remove from connected clients list
        var connectionId = networkData.ConnectionId;

        try
        {
            connectedClientsIds.Remove(connectionId);
            connectedClientsNames.Remove(connectionId);    
        }
        catch (Exception ex)
        {
            
        }

        if (OnClientDisconnected != null)
        {
            OnClientDisconnected(this, new ClientConnectionDataEventArgs(connectionId));    
        }
    }

    void FilterCommandLineOptions(NetworkCommandData command)
    {
        command.Options.ToList()
            .Where(o => o.Key == "ConnectionId")
            .ToList()
            .ForEach(o => command.RemoveOption(o.Key));
    }

    void OnClientSendData(NetworkData receiveNetworkData)
    {
        //if we received data from client
        var connectionId = receiveNetworkData.ConnectionId;
        var message = receiveNetworkData.Message;
        NetworkCommandData commmandData = null;

        try
        {
            commmandData = NetworkCommandData.Parse(message);
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }

        if (commmandData != null)
        {
            FilterCommandLineOptions(commmandData);
            commmandData.AddOption("ConnectionId", connectionId.ToString());

            try
            {
                commandsManager.Execute(commmandData);    
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }
        else
        {
            var username = GetClientUsername(connectionId); 

            if (OnReceivedData != null)
            {
                OnReceivedData(this, new DataSentEventArgs(receiveNetworkData.ConnectionId, username, message));    
            }
        }
    }

    public string GetClientUsername(int connectionId)
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

    public void SetClientName(int connectionId, string name)
    {
        if (!connectedClientsIds.Contains(connectionId))
        {
            throw new ArgumentException("Client with id " + connectionId + " is not connected");
        } 

        connectedClientsNames.Add(connectionId, name);
    }

    public void SendClientCommand(int connectionId, NetworkCommandData command)
    {
        SendClientMessage(connectionId, command.ToString());
    }

    public void SendClientMessage(int connectionId, string message)
    {
        NetworkTransportUtils.SendMessageAsync(genericHostId, connectionId, communicationChannel, message, (exception) =>
            {
                var errorN = exception.ErrorN;
                var error = (NetworkError)errorN;
                var errorMessage = NetworkErrorUtils.GetMessage(error);

                NotificationServiceController.AddNotification(Color.red, errorMessage);
            });
    }

    public void SendAllClientsCommand(NetworkCommandData command)
    {
        SendAllClientsMessage(command.ToString());
    }

    public void SendAllClientsCommand(NetworkCommandData command, int exceptConnectionId)
    {
        SendAllClientsMessage(command.ToString(), exceptConnectionId);
    }

    public void SendAllClientsMessage(string message)
    {
        for (int i = 0; i < connectedClientsIds.Count; i++)
        {
            var clientId = connectedClientsIds[i];
            SendClientMessage(clientId, message);
        }
    }

    public void SendAllClientsMessage(string message, int exceptConnectionId)
    {
        for (int i = 0; i < connectedClientsIds.Count; i++)
        {
            var clientId = connectedClientsIds[i];

            if (clientId == exceptConnectionId)
            {
                continue;
            }

            SendClientMessage(clientId, message);
        }
    }

    public void KickPlayer(int connectionId, string message)
    {
        if (!IsClientConnected(connectionId))
        {
            throw new Exception("Client with id " + connectionId + " not connected"); 
        }   

        try
        {
            var commandLine = new NetworkCommandData("ShowNotification");
            commandLine.AddOption("Color", "red");
            commandLine.AddOption("Message", message);

            SendClientCommand(connectionId, commandLine);    
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
        if (bannedConnections.Contains(connectionId))
        {
            return;
        }

        bannedConnections.Add(connectionId);
        KickPlayer(connectionId, "Нямаш право да влизаш във сървъра.");
    }

    #region DEBUG

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

        var allClientsIdsNames = connectedClientsNames.ToList();

        for (int i = 0; i < allClientsIdsNames.Count; i++)
        {
            var clientId = allClientsIdsNames[i].Key;
            var clientUsername = allClientsIdsNames[i].Value;
            var width = 100;
            var height = 30;
            var x = connectedPlayersRect.position.x + 5;
            var y = kickRandomPlayerButtonRect.position.y + kickRandomPlayerButtonRect.height + 5 + ((i * height) + 5);
            var rect = new Rect(x, y, width, height);

            GUI.Label(rect, string.Format("{0} {1}", clientId, clientUsername));
        }
    }

    #endregion
}


