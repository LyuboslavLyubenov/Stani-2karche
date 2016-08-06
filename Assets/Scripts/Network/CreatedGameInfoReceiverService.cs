using UnityEngine;
using System.Collections.Generic;
using System;

public class CreatedGameInfoReceiverService : MonoBehaviour
{
    public P2PSocket P2PSocket;

    private readonly object MyLock = new object();

    Dictionary<string, Action<GameInfoReceivedDataEventArgs>> pendingIPReceiveDataJson = new Dictionary<string, Action<GameInfoReceivedDataEventArgs>>();

    void Start()
    {
        P2PSocket.OnReceivedMessage += OnReceivedDataFromClient;
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
        GameInfoReceivedDataEventArgs data = null;

        try
        {
            data = new GameInfoReceivedDataEventArgs(filteredMessage);    
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }

        pendingIPReceiveDataJson[args.IPAddress](data);
    }

    public void ListenAt(string ip, Action<GameInfoReceivedDataEventArgs> onReceived)
    {
        lock (MyLock)
        {
            if (!pendingIPReceiveDataJson.ContainsKey(ip))
            {
                P2PSocket.ConnectTo(ip, P2PSocket.Port, (IpEventArgs args) => OnConnected(args, onReceived));
            }
        }
    }

    void OnConnected(IpEventArgs args, Action<GameInfoReceivedDataEventArgs> onReceived)
    {
        pendingIPReceiveDataJson.Add(args.IPAddress, onReceived);
    }

    public void RemoveListener(string ip)
    {
        lock (MyLock)
        {
            if (pendingIPReceiveDataJson.ContainsKey(ip))
            {
                pendingIPReceiveDataJson.Remove(ip);
            }
        }
    }

    public bool IsListening(string ip)
    {
        lock (MyLock)
        {
            return pendingIPReceiveDataJson.ContainsKey(ip);
        }
    }
    //*/
}