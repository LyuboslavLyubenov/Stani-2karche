using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections.Generic;
using System.Collections;

public class ServerNetworkManager : MonoBehaviour
{
    const int port = 7788;

    public int MaxConnections;

    int genericHostId = 0;
    ConnectionConfig connectionConfig = null;
    HostTopology topology = null;
    byte communicationChannel = 0;

    bool isRunning = false;

    List<int> connectedClientsId = new List<int>();
    Dictionary<int, string> connectedClientsNames = new Dictionary<int, string>();

    GameData gameData = null;

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
            return connectedClientsId.Count;    
        }
    }

    public IList<int> ConnectedClientsId
    {
        get
        {
            return connectedClientsId;    
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
        #if UNITY_EDITOR
        Application.runInBackground = true;
        #endif

        gameData = GameObject.FindWithTag("MainCamera").GetComponent<GameData>();

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
                    Debug.Log(e.Message);
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

                                var username = message.Substring(usernameDelimeterIndex + 1, message.Length - usernameDelimeterIndex);

                                if (!string.IsNullOrEmpty(username) &&
                                    username.Length >= 4 &&
                                    !connectedClientsNames.ContainsKey(connectionId))
                                {
                                    connectedClientsNames.Add(connectionId, username);
                                }
                            }
                            else
                            {

                                var username = "";

                                if (connectedClientsNames.ContainsKey(connectionId))
                                {
                                    username = connectedClientsNames[connectionId];
                                }
                                else
                                {
                                    username = "Player " + connectionId;
                                }

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
        genericHostId = NetworkTransport.AddHost(topology, port, null);
        isRunning = true;
    }

    public void StopServer()
    {
        NetworkTransport.Shutdown();
        isRunning = false;
    }

    public void SendClientMessage(int clientId, string message)
    {
        NetworkTransportUtils.SendMessage(genericHostId, clientId, communicationChannel, message);
    }
}
