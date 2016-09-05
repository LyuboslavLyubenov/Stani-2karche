using UnityEngine;
using System;
using System.Collections.Generic;

public abstract class GameDataGetQuestionAbstractCommand : INetworkManagerCommand
{
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

    protected void SendQuestion(int connectionId, Question question, QuestionRequestType requestType)
    {
        var questionJSON = JsonUtility.ToJson(question);
        var commandData = new NetworkCommandData("GameDataQuestion");
        var requestTypeStr = Enum.GetName(typeof(QuestionRequestType), requestType);
        commandData.AddOption("QuestionJSON", questionJSON);
        commandData.AddOption("RemainingQuestionsToNextMark", GameData.RemainingQuestionsToNextMark.ToString());
        commandData.AddOption("RequestType", requestTypeStr);
        NetworkManager.SendClientCommand(connectionId, commandData);
    }

    public abstract void Execute(Dictionary<string, string> commandsOptionsValues);
}