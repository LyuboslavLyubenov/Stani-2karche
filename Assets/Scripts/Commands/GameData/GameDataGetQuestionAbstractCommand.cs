using UnityEngine;
using System;
using System.Collections.Generic;

public abstract class GameDataGetQuestionAbstractCommand : INetworkManagerCommand
{
    public EventHandler<ServerSentQuestionEventArgs> OnSentQuestion = delegate
    {
    };

    protected ServerNetworkManager NetworkManager
    {
        get;
        private set;
    }

    protected LocalGameData GameData
    {
        get;
        private set;
    }

    protected GameDataGetQuestionAbstractCommand(LocalGameData gameData, ServerNetworkManager networkManager)
    {
        if (gameData == null)
        {
            throw new ArgumentNullException("gameData");
        }

        if (networkManager == null)
        {
            throw new ArgumentNullException("networkManager");
        }

        this.NetworkManager = networkManager;
        this.GameData = gameData;
    }

    protected void SendQuestion(int connectionId, ISimpleQuestion question, QuestionRequestType requestType)
    {
        var questionJSON = JsonUtility.ToJson(question.Serialize());
        var commandData = new NetworkCommandData("GameDataQuestion");
        var requestTypeStr = Enum.GetName(typeof(QuestionRequestType), requestType);
        commandData.AddOption("QuestionJSON", questionJSON);
        commandData.AddOption("RemainingQuestionsToNextMark", GameData.RemainingQuestionsToNextMark.ToString());
        commandData.AddOption("SecondsForAnswerQuestion", GameData.SecondsForAnswerQuestion.ToString());
        commandData.AddOption("RequestType", requestTypeStr);
        NetworkManager.SendClientCommand(connectionId, commandData);
        OnSentQuestion(this, new ServerSentQuestionEventArgs(question, requestType, connectionId));
    }

    public abstract void Execute(Dictionary<string, string> commandsOptionsValues);
}