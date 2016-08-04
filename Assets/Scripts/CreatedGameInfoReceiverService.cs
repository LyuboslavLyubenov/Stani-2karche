using UnityEngine;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using CielaSpike;
using System.Net;
using System;
using System.Text;
using System.Linq;
using System.Threading;
using System.Security.Cryptography;

public class CreatedGameInfoReceiverService : SimpleTcpServer
{
    const int Port = 7773;

    Dictionary<string, Action<GameInfoReceivedData>> pendingIPReceiveDataJson = new Dictionary<string, Action<GameInfoReceivedData>>();

    void Start()
    {
        base.Initialize(Port);
        base.OnClientConnected += OnConnectedClient;
        base.OnReceivedMessage += OnReceivedDataFromClient;
    }

    void OnConnectedClient(object sender, IpEventArgs args)
    {
        lock (pendingIPReceiveDataJson)
        {
            var ip = args.IPAddress;

            if (!pendingIPReceiveDataJson.ContainsKey(ip))
            {
                base.DisconnectClient(ip);
            }
        }
    }

    void OnReceivedDataFromClient(object sender, MessageEventArgs args)
    {
        const string GameInfoTag = "[CreatedGameInfo]";

        if (!pendingIPReceiveDataJson.ContainsKey(args.IPAddress) ||
            !args.Message.Contains(GameInfoTag))
        {
            return;
        }

        var filteredMessage = args.Message.Replace(GameInfoTag, ""); 
        GameInfoReceivedData data = null;

        try
        {
            data = new GameInfoReceivedData(filteredMessage);    
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }

        pendingIPReceiveDataJson[args.IPAddress](data);
        base.DisconnectClient(args.IPAddress);    
    }

    public void ListenAt(string ip, Action<GameInfoReceivedData> onReceived)
    {
        lock (pendingIPReceiveDataJson)
        {
            if (!pendingIPReceiveDataJson.ContainsKey(ip))
            {
                pendingIPReceiveDataJson.Add(ip, onReceived);    
            }
        }

        Debug.Log("ListeningAt " + ip);
    }

    public void RemoveListener(string ip)
    {
        lock (pendingIPReceiveDataJson)
        {
            if (pendingIPReceiveDataJson.ContainsKey(ip))
            {
                pendingIPReceiveDataJson.Remove(ip);
            }
        }
    }

    public bool IsListening(string ip)
    {
        lock (pendingIPReceiveDataJson)
        {
            return pendingIPReceiveDataJson.ContainsKey(ip);
        }
    }
    //*/
}