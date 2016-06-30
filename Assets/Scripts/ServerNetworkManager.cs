using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections.Generic;
using System.Collections;

public class ServerNetworkManager : MonoBehaviour, INetworkManager
{
    const int Port = 7788;
    //how many clients can be connected to the server
    public int MaxConnections;
    public GameObject DialogUI;
    public LANBroadcastService broadcastService;

    DialogUIController dialogUIController = null;

    int genericHostId = 0;

    ConnectionConfig connectionConfig = null;
    HostTopology topology = null;

    byte communicationChannel = 0;

    bool isRunning = false;
    //Id of all connected clients
    List<int> connectedClientsId = new List<int>();
    //Their names
    Dictionary<int, string> connectedClientsNames = new Dictionary<int, string>();

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

    public EventHandler OnConnectedEvent
    {
        get;
        set;
    }

    public EventHandler<DataSentEventArgs> OnReceivedDataEvent
    {
        get;
        set;
    }

    public EventHandler OnDisconnectedEvent
    {
        get;
        set;
    }

    void Start()
    {
        if (DialogUI != null)
        {
            dialogUIController = DialogUI.GetComponent<DialogUIController>();
        }

        ConfigureServer();
        StartServer();

        StartCoroutine(UpdateCoroutine());
    }

    void ConfigureServer()
    {
        connectionConfig = new ConnectionConfig();
        communicationChannel = connectionConfig.AddChannel(QosType.ReliableSequenced);//make sure all messages are in order and received
        topology = new HostTopology(connectionConfig, MaxConnections);
    }

    IEnumerator UpdateCoroutine()
    {
        while (true)
        {
            if (isRunning)
            {
                NetworkData recieveNetworkData = null;
                bool hasError = false;

                try
                {
                    recieveNetworkData = NetworkTransportUtils.ReceiveMessage();
                }
                catch (NetworkException e)
                {
                    if (dialogUIController != null)
                    {
                        var errorMessage = (NetworkError)e.ErrorN;
                        DialogUI.SetActive(true);
                        dialogUIController.SetErrorMessage(errorMessage);
                    }

                    hasError = true;
                }

                if (!hasError)
                {
                    //client id
                    var connectionId = recieveNetworkData.ConnectionId;

                    switch (recieveNetworkData.NetworkEventType)
                    {
                        case NetworkEventType.ConnectEvent:
                            connectedClientsId.Add(recieveNetworkData.ConnectionId);

                            if (OnConnectedEvent != null)
                            {
                                OnConnectedEvent(this, EventArgs.Empty);    
                            }

                            break;

                        case NetworkEventType.BroadcastEvent:
                            break;

                        case NetworkEventType.DataEvent:
                            //if we received data from client
                            var message = recieveNetworkData.Message;
                            var usernameSetCommandIndex = message.IndexOf("UsernameSet");

                            //is user trying to send username
                            if (usernameSetCommandIndex > -1)
                            {
                                //yes!
                                var usernameDelimeterIndex = message.IndexOf("=");

                                if (usernameDelimeterIndex <= usernameSetCommandIndex)
                                {
                                    //empty username :(
                                    connectedClientsNames[connectionId] = "Играч Номер " + connectionId;
                                }

                                var username = message.Substring(usernameDelimeterIndex + 1, message.Length - usernameDelimeterIndex - 1);

                                if (!string.IsNullOrEmpty(username) &&
                                    username.Length >= 4)
                                {
                                    connectedClientsNames[connectionId] = username;
                                }
                                else
                                {
                                    connectedClientsNames[connectionId] = "Играч Номер " + connectionId;
                                }
                            }
                            else
                            {
                                //its not asking to set username
                                var username = connectedClientsNames[connectionId];

                                if (OnReceivedDataEvent != null)
                                {
                                    OnReceivedDataEvent(this, new DataSentEventArgs(recieveNetworkData.ConnectionId, username, message));    
                                }
                            }

                            break;

                        case NetworkEventType.DisconnectEvent:
                            //if disconnected remove from connected clients list
                            connectedClientsId.Remove(connectionId);
                            connectedClientsNames.Remove(connectionId);

                            if (OnDisconnectedEvent != null)
                            {
                                OnDisconnectedEvent(this, EventArgs.Empty);    
                            }

                            break;
                    }    
                }
            }

            yield return new WaitForEndOfFrame();
        }
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
    public void SendMessage(int clientId, string message)
    {
        NetworkTransportUtils.SendMessage(genericHostId, clientId, communicationChannel, message);
    }

    public void CallFriend(Question question, int friendId)
    {
        var questionJSON = JsonUtility.ToJson(question);

        SendMessage(friendId, "AskFriend");
        SendMessage(friendId, questionJSON);
    }

    public void AskAudience(Question question)
    {
        var questionJSON = JsonUtility.ToJson(question);

        for (int i = 0; i < connectedClientsId.Count; i++)
        {
            var clientConnectionId = connectedClientsId[i];

            SendMessage(clientConnectionId, "AskAudience");
            SendMessage(clientConnectionId, questionJSON);
        }
    }
}
