﻿using UnityEngine;
using System;
using System.Collections.Generic;

public class CreatedGameInfoReceiverService : MonoBehaviour
{
    public SimpleTcpClient TcpClient;
    public SimpleTcpServer TcpServer;

    public Dictionary<string, Action<GameInfoReceivedDataEventArgs>> pendingRequests = new Dictionary<string, Action<GameInfoReceivedDataEventArgs>>();

    // Use this for initialization
    void Start()
    {
        if (!TcpClient.Initialized)
        {
            TcpClient.Initialize();
        }

        if (!TcpServer.Initialized)
        {
            TcpServer.Initialize(7774);
        }

        TcpServer.OnReceivedMessage += OnReceivedMessage;
    }

    void OnReceivedMessage(object sender, MessageEventArgs args)
    {
        var gameInfoTagIndex = args.Message.IndexOf(CreatedGameInfoSenderService.GameInfoTag);

        if (!pendingRequests.ContainsKey(args.IPAddress) || gameInfoTagIndex < 0)
        {
            return;
        }

        var filteredMessage = args.Message.Remove(gameInfoTagIndex, CreatedGameInfoSenderService.GameInfoTag.Length);
        var gameInfo = new GameInfoReceivedDataEventArgs(filteredMessage);

        pendingRequests[args.IPAddress](gameInfo);
        pendingRequests.Remove(args.IPAddress);
    }

    public void ReceiveFrom(string ipAddress, Action<GameInfoReceivedDataEventArgs> receivedGameInfo)
    {
        TcpClient.ConnectTo(ipAddress, TcpServer.Port, () =>
            {
                TcpClient.Send(ipAddress, CreatedGameInfoSenderService.SendGameInfoCommandTag);
                pendingRequests.Add(ipAddress, receivedGameInfo);
            });
    }

    public void StopReceivingFrom(string ipAddress)
    {
        if (!pendingRequests.ContainsKey(ipAddress))
        {
            throw new InvalidOperationException("Not listening to this ipAddress");
        }

        pendingRequests.Remove(ipAddress);
        TcpClient.DisconnectFrom(ipAddress);
        TcpServer.Disconnect(ipAddress);
    }
}
