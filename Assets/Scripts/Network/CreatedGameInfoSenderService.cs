using UnityEngine;
using System;

public class CreatedGameInfoSenderService : ExtendedMonoBehaviour
{
    const string GameInfoTag = "[CreatedGameInfo]";
    const string SendGameInfoTag = "[SendGameInfo]";

    public P2PSocket P2PSocket;
    public GameInfoFactory GameInfoFactory;

    void Start()
    {
        P2PSocket.OnReceivedMessage += OnReceivedMessage;
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

        P2PSocket.Send(args.IPAddress, messageToSend);
    }
}

