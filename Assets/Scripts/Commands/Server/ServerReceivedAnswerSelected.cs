using System.Collections.Generic;
using System;
using UnityEngine;

public class ServerReceivedAnswerSelected : INetworkManagerCommand
{
    IGameData gameData;
    ServerNetworkManager networkManager;

    public ServerReceivedAnswerSelected(IGameData gameData, ServerNetworkManager networkManager)
    {
        if (gameData == null)
        {
            throw new ArgumentNullException("gameData");
        }
            
        if (networkManager == null)
        {
            throw new ArgumentNullException("networkManager");
        }
            
        this.gameData = gameData;
        this.networkManager = networkManager;
    }

    public void Execute(Dictionary<string, string> commandsOptionsValues)
    {
        var connectionId = int.Parse(commandsOptionsValues["ClientToSendConnectionId"]);
        var answerSelected = commandsOptionsValues["Answer"];

        gameData.GetCurrentQuestion((question) => SendAnswerCorrectStatus(question, connectionId, answerSelected), Debug.LogException);
    }

    void SendAnswerCorrectStatus(Question currentQuestion, int connectionId, string answerSelected)
    {
        var correctAnswer = currentQuestion.Answers[currentQuestion.CorrectAnswerIndex];
        var commandData = new NetworkCommandData("AnswerSelected");
        commandData.AddOption("IsCorrect", (answerSelected == correctAnswer).ToString());
        networkManager.SendClientCommand(connectionId, commandData);
    }
}
