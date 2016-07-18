using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public class ServerNetworkManager : MonoBehaviour, INetworkManager
{
    const int Port = 7788;
    //how many clients can be connected to the server
    public int MaxConnections;
    public NotificationsController NotificationServiceController;
    public LANBroadcastService BroadcastService;

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
    List<int> connectedClientsId = new List<int>();
    List<int> bannedConnections = new List<int>();
    //Their names
    Dictionary<int, string> connectedClientsNames = new Dictionary<int, string>();
    Dictionary<string, Action<NetworkData>> commands = new Dictionary<string, Action<NetworkData>>();

    public bool IsRunning
    {
        get
        {
            return isRunning;
        }
    }

    public IList<int> ConnectedClientsId
    {
        get
        {
            return connectedClientsId;    
        }
    }

    public Dictionary<int, string> ConnectedClientsNames
    {
        get
        {
            return connectedClientsNames;
        }
    }

    void Start()
    {
        ConfigureServer();
        StartServer();

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

    void InitializeCommands()
    {
        commands["SetUsername"] = SetUsernameCommand;
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

            yield return new WaitForEndOfFrame();
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

    void OnClientConnected(NetworkData networkData)
    {
        var connectionId = networkData.ConnectionId;
        connectedClientsId.Add(connectionId);

        var isBanned = bannedConnections.IndexOf(connectionId) > -1;

        if (isBanned)
        {
            KickPlayer(connectionId, "Забранено ти е да влизаш във този сървър.");
            return;
        }

        if (OnConnectedEvent != null)
        {
            OnConnectedEvent(this, EventArgs.Empty);    
        }
    }

    void OnClientDisconnect(NetworkData networkData)
    {
        //if disconnected remove from connected clients list
        var connectionId = networkData.ConnectionId;

        connectedClientsId.Remove(connectionId);
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
        var isValidCommand = (!string.IsNullOrEmpty(commandName) && commands.ContainsKey(commandName));

        if (isValidCommand)
        {
            var commandToExecute = commands[commandName];
            commandToExecute.Invoke(receiveNetworkData);
        }
        else
        {
            var username = connectedClientsNames[connectionId];

            if (OnReceivedDataEvent != null)
            {
                OnReceivedDataEvent(this, new DataSentEventArgs(receiveNetworkData.ConnectionId, username, message));    
            }
        }
    }

    void SetUsernameCommand(NetworkData networkData)
    {
        var commandParams = networkData.Message.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries).Skip(1).ToArray();
        var connectionId = networkData.ConnectionId;

        if (commandParams.Length < 0 || string.IsNullOrEmpty(commandParams[0]))
        {
            //empty username :(
            connectedClientsNames[connectionId] = "Играч Номер " + connectionId;
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

    bool IsClientConnected(int connectionId)
    {
        return connectedClientsId.IndexOf(connectionId) > -1;
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

    //useful if you want to send individual client a message
    public void SendClientMessage(int clientId, string message)
    {
        NetworkTransportUtils.SendMessage(genericHostId, clientId, communicationChannel, message);
    }

    public void SendAllClientsMessage(string message)
    {
        for (int i = 0; i < connectedClientsId.Count; i++)
        {
            var clientId = connectedClientsId[i];
            SendClientMessage(clientId, message);
        }
    }

    public void CallFriend(Question question, int friendId)
    {
        var questionJSON = JsonUtility.ToJson(question);

        SendClientMessage(friendId, "AskFriend");
        SendClientMessage(friendId, questionJSON);
    }

    public void AskAudience(Question question)
    {
        var questionJSON = JsonUtility.ToJson(question);

        for (int i = 0; i < connectedClientsId.Count; i++)
        {
            var clientConnectionId = connectedClientsId[i];

            SendClientMessage(clientConnectionId, "AskAudience");
            SendClientMessage(clientConnectionId, questionJSON);
        }
    }

    public void KickPlayer(int connectionId, string message)
    {
        if (!IsClientConnected(connectionId))
        {
            return;    
        }   

        SendClientMessage(connectionId, "KickReason=" + message);

        byte error;
        NetworkTransport.Disconnect(genericHostId, connectionId, out error);
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
}