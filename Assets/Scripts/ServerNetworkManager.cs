﻿using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Security.Cryptography;

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
        #if UNITY_EDITOR_WIN
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
                            OnClientSentMessage(this, new DataSentEventArgs(recieveNetworkData.ConnectionId, message));
                            break;

                        case NetworkEventType.DisconnectEvent:
                            connectedClientsId.Remove(recieveNetworkData.ConnectionId);
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
