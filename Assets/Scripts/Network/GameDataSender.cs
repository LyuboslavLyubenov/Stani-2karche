﻿using UnityEngine;
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
        LocalGameData.OnLoaded += (sender, args) => SendLoadedGameData();

        var getCurrentQuestionCommand = new ReceivedGetCurrentQuestionCommand(LocalGameData, NetworkManager);
        var getRandomQuestionCommand = new ReceivedGetRandomQuestionCommand(LocalGameData, NetworkManager);
        var getNextQuestionCommand = new ReceivedGetNextQuestionCommand(LocalGameData, NetworkManager);
                
        getCurrentQuestionCommand.OnBeforeSend += OnBeforeSentToClient;
        getRandomQuestionCommand.OnBeforeSend += OnBeforeSentToClient;
        getNextQuestionCommand.OnBeforeSend += OnBeforeSentToClient;

        getCurrentQuestionCommand.OnSentQuestion += OnSentQuestionToClient;
        getRandomQuestionCommand.OnSentQuestion += OnSentQuestionToClient;
        getNextQuestionCommand.OnSentQuestion += OnSentQuestionToClient;

        NetworkManager.CommandsManager.AddCommand("GameDataGetQuestion", new GameDataGetQuestionRouterCommand(NetworkManager));
        NetworkManager.CommandsManager.AddCommand("GameDataGetCurrentQuestion", getCurrentQuestionCommand);
        NetworkManager.CommandsManager.AddCommand("GameDataGetRandomQuestion", getRandomQuestionCommand);
        NetworkManager.CommandsManager.AddCommand("GameDataGetNextQuestion", getNextQuestionCommand);
    }

    void OnClientConnected(object sender, ClientConnectionDataEventArgs args)
    {
        if (LocalGameData.Loaded)
        {
            SendLoadedGameData();
        }
    }

    void OnMarkIncrease(object sender, MarkEventArgs args)
    {
        var commandData = new NetworkCommandData("GameDataMark");
        commandData.AddOption("Mark", args.Mark.ToString());

        NetworkManager.SendAllClientsCommand(commandData);
    }

    void SendLoadedGameData()
    {
        var loadedGameDataCommand = new NetworkCommandData("LoadedGameData");
        NetworkManager.SendAllClientsCommand(loadedGameDataCommand);
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