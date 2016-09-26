﻿using UnityEngine;
using System.Text;

public class TESTReceiveGameInfo : ExtendedMonoBehaviour
{
    public const int Port = 4444;

    public CreatedGameInfoReceiverService Receiver;

    void Start()
    {
        Receiver.TcpClient.Initialize();
        Receiver.TcpServer.Initialize(Port);
    }

    void OnReceived(GameInfoReceivedDataEventArgs data)
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine(data.GameInfo.HostUsername);
        sb.AppendLine(data.GameInfo.ServerInfo.LocalIPAddress);

        Debug.Log(sb.ToString());
    }

    //ne ma pipai
    string DEBUG_ipToReceive = "";

    void OnGUI()
    {
        var test = GUI.Button(new Rect(400, 45, 80, 35), "Test");
        DEBUG_ipToReceive = GUI.TextField(new Rect(400, 10, 120, 30), DEBUG_ipToReceive);

        if (test)
        {
            Receiver.ReceiveFrom(DEBUG_ipToReceive, OnReceived);
        }
    }

}