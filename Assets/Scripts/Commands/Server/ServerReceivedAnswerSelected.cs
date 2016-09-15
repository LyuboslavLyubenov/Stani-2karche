using System.Collections.Generic;
using System;
using UnityEngine;

public class ServerReceivedAnswerSelected : INetworkManagerCommand
{
    public delegate void OnReceivedAnswerDelegate(int clientId,string answer);

    IGameData gameData;
    ServerNetworkManager networkManager;
    OnReceivedAnswerDelegate onReceivedAnswer;

    public ServerReceivedAnswerSelected(IGameData gameData, ServerNetworkManager networkManager, OnReceivedAnswerDelegate onReceivedAnswer)
    {
        if (gameData == null)
        {
            throw new ArgumentNullException("gameData");
        }
            
        if (networkManager == null)
        {
            throw new ArgumentNullException("networkManager");
        }

        if (onReceivedAnswer == null)
        {
            throw new ArgumentNullException("onReceivedAnswer");
        }
            
        this.gameData = gameData;
        this.networkManager = networkManager;
        this.onReceivedAnswer = onReceivedAnswer;
    }

    public virtual void Execute(Dictionary<string, string> commandsOptionsValues)
    {
        var connectionId = int.Parse(commandsOptionsValues["ConnectionId"]);
        var answerSelected = commandsOptionsValues["Answer"];

        onReceivedAnswer(connectionId, answerSelected);
        //TODO:
    }
}

public class ServerReceivedAnswerSelectedOneTime : ServerReceivedAnswerSelected, IOneTimeExecuteCommand
{
    public bool FinishedExecution
    {
        get;
        private set;
    }

    public ServerReceivedAnswerSelectedOneTime(IGameData gameData, ServerNetworkManager networkManager, OnReceivedAnswerDelegate onReceivedAnswer)
        : base(gameData, networkManager, onReceivedAnswer)
    {
        FinishedExecution = false;
    }

    public override void Execute(Dictionary<string, string> commandsOptionsValues)
    {
        base.Execute(commandsOptionsValues);
        FinishedExecution = true;
    }
}