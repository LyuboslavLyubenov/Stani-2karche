using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections.Generic;
using System.Collections;

public class ServerNetworkManager : MonoBehaviour
{
    const int Port = 7788;

    public int MaxConnections;
    public GameObject DialogUI;
    public LANBroadcastService broadcastService;

    DialogUIController dialogUIController = null;

    int genericHostId = 0;

    ConnectionConfig connectionConfig = null;
    HostTopology topology = null;

    byte communicationChannel = 0;

    bool isRunning = false;

    List<int> connectedClientsId = new List<int>();
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

    public EventHandler OnClientConnect = delegate
    {
    };

    public EventHandler OnClientDisconnect = delegate
    {
    };

    public EventHandler<DataSentEventArgs> OnClientBroadcast = delegate
    {
    };

    public EventHandler<DataSentEventArgs> OnClientSentMessage = delegate
    {  
    };

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
        communicationChannel = connectionConfig.AddChannel(QosType.ReliableSequenced);
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
                    recieveNetworkData = NetworkTransportUtils.RecieveMessage();
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
                    var connectionId = recieveNetworkData.ConnectionId;

                    switch (recieveNetworkData.NetworkEventType)
                    {
                        case NetworkEventType.ConnectEvent:
                            connectedClientsId.Add(recieveNetworkData.ConnectionId);
                            OnClientConnect(this, EventArgs.Empty);
                            break;

                        case NetworkEventType.BroadcastEvent:
                            break;

                        case NetworkEventType.DataEvent:
                            var message = recieveNetworkData.Message;
                            var usernameSetCommandIndex = message.IndexOf("UsernameSet");

                            if (usernameSetCommandIndex > -1)
                            {
                                var usernameDelimeterIndex = message.IndexOf("=");

                                if (usernameDelimeterIndex <= usernameSetCommandIndex)
                                {
                                    break;
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
                                var username = connectedClientsNames[connectionId];
                                OnClientSentMessage(this, new DataSentEventArgs(recieveNetworkData.ConnectionId, username, message));
                            }

                            break;

                        case NetworkEventType.DisconnectEvent:
                            connectedClientsId.Remove(connectionId);
                            connectedClientsNames.Remove(connectionId);
                            OnClientDisconnect(this, EventArgs.Empty);
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

    public void SendClientMessage(int clientId, string message)
    {
        NetworkTransportUtils.SendMessage(genericHostId, clientId, communicationChannel, message);
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
}
