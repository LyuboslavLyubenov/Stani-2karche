using UnityEngine;
using System;

public class CreatedGameInfoSenderService : ExtendedMonoBehaviour
{
    const string GameInfoTag = "[CreatedGameInfo]";
    const string SendGameInfoTag = "[SendGameInfo]";

    public SimpleTcpServer TcpServer;
    public GameInfoFactory GameInfoFactory;

    void Start()
    {
        TcpServer.OnReceivedMessage += OnReceivedMessage;
    }

    void OnReceivedMessage(object sender, MessageEventArgs args)
    {
        if (!args.Message.Contains(SendGameInfoTag))
        {   
            return;
        }

        var filteredMessage = args.Message.Replace(SendGameInfoTag, "");
        var gameInfo = GameInfoFactory.Get();
        var gameInfoJSON = JsonUtility.ToJson(gameInfo);
        var messageToSend = GameInfoTag + gameInfoJSON;

        TcpServer.Send(args.IPAddress, messageToSend);
    }
}

