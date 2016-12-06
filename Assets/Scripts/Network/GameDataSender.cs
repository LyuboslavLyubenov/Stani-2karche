using UnityEngine;
using System;

public class GameDataSender : MonoBehaviour
{
    public EventHandler<ServerSentQuestionEventArgs> OnBeforeSend = delegate
    {
    };

    public EventHandler<ServerSentQuestionEventArgs> OnSentQuestion = delegate
    {
    };
    
    public LocalGameData LocalGameData;
    public ServerNetworkManager NetworkManager;

    void Start()
    {
        NetworkManager.OnClientConnected += OnClientConnected;
        LocalGameData.OnMarkIncrease += OnMarkIncrease;
        LocalGameData.OnLoaded += OnGameDataLoaded;

        var getCurrentQuestionCommand = new ReceivedGetCurrentQuestionCommand(LocalGameData, NetworkManager);
        var getRandomQuestionCommand = new ReceivedGetRandomQuestionCommand(LocalGameData, NetworkManager);
        var getNextQuestionCommand = new ReceivedGetNextQuestionCommand(LocalGameData, NetworkManager);
                
        getCurrentQuestionCommand.OnBeforeSend += OnBeforeSentToClient;
        getRandomQuestionCommand.OnBeforeSend += OnBeforeSentToClient;
        getNextQuestionCommand.OnBeforeSend += OnBeforeSentToClient;

        getCurrentQuestionCommand.OnSentQuestion += OnSentQuestionToClient;
        getRandomQuestionCommand.OnSentQuestion += OnSentQuestionToClient;
        getNextQuestionCommand.OnSentQuestion += OnSentQuestionToClient;

        var commandsManager = NetworkManager.CommandsManager;

        commandsManager.AddCommand("GameDataGetQuestion", new GameDataGetQuestionRouterCommand(NetworkManager));
        commandsManager.AddCommand("GameDataGetCurrentQuestion", getCurrentQuestionCommand);
        commandsManager.AddCommand("GameDataGetRandomQuestion", getRandomQuestionCommand);
        commandsManager.AddCommand("GameDataGetNextQuestion", getNextQuestionCommand);
    }

    void OnGameDataLoaded(object sender, EventArgs args)
    {
        var connectedClients = NetworkManager.ConnectedClientsConnectionId;

        for (int i = 0; i < connectedClients.Length; i++)
        {
            var clientId = connectedClients[i];
            SendLoadedGameData(clientId);
        }
    }

    void OnClientConnected(object sender, ClientConnectionDataEventArgs args)
    {
        if (LocalGameData.Loaded)
        {
            SendLoadedGameData(args.ConnectionId);
        }
    }

    void OnMarkIncrease(object sender, MarkEventArgs args)
    {
        var commandData = new NetworkCommandData("GameDataMark");
        commandData.AddOption("Mark", args.Mark.ToString());
        NetworkManager.SendAllClientsCommand(commandData);
    }

    void SendLoadedGameData(int connectionId)
    {
        var loadedGameDataCommand = new NetworkCommandData("LoadedGameData");
        loadedGameDataCommand.AddOption("LevelCategory", LocalGameData.LevelCategory);
        NetworkManager.SendClientCommand(connectionId, loadedGameDataCommand);
    }

    void OnSentQuestionToClient(object sender, ServerSentQuestionEventArgs args)
    {
        OnSentQuestion(this, args);
    }

    void OnBeforeSentToClient(object sender, ServerSentQuestionEventArgs args)
    {
        OnBeforeSend(this, args);
    }
}