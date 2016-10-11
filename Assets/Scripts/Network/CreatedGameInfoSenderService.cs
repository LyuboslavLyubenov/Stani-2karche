using UnityEngine;
using System;

public class CreatedGameInfoSenderService : ExtendedMonoBehaviour
{
    public const string GameInfoTag = "[CreatedGameInfo]";
    public const string SendGameInfoCommandTag = "[Command:SendGameInfo]";

    public SimpleTcpClient TcpClient;
    public SimpleTcpServer TcpServer;
    public GameInfoFactory GameInfoFactory;

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

        Debug.Log("Created game info inizialized");
    }

    void OnReceivedMessage(object sender, MessageEventArgs args)
    {
        if (!args.Message.Contains(SendGameInfoCommandTag))
        {   
            return;
        }

        var gameInfo = GameInfoFactory.Get("BasicExam");
        var gameInfoJSON = JsonUtility.ToJson(gameInfo);
        var messageToSend = GameInfoTag + gameInfoJSON;

        TcpClient.ConnectTo(args.IPAddress, TcpServer.Port, () => TcpClient.Send(args.IPAddress, messageToSend));

        Debug.Log("Sending game info to " + args.IPAddress);
    }
}

