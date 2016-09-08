using System;
using System.Collections.Generic;
using UnityEngine;

public class LoadQuestionCommand : INetworkManagerCommand
{
    Action<QuestionEventArgs> OnReceivedQuestion;

    GameState currentGameState;

    public LoadQuestionCommand(Action<QuestionEventArgs> OnReceivedQuestion, GameState currentGameState)
    {
        if (OnReceivedQuestion == null)
        {
            throw new ArgumentNullException("OnReceivedQuestion");
        }
            
        this.OnReceivedQuestion = OnReceivedQuestion;
        this.currentGameState = currentGameState;
    }

    public void Execute(Dictionary<string, string> commandsOptionsValues)
    {
        if (currentGameState == GameState.Idle)
        {
            throw new Exception("Cannot execute command in this game state (idle)");
        }

        var questionJSON = commandsOptionsValues["QuestionJSON"];
        var question = JsonUtility.FromJson<Question>(questionJSON);
        var questionEventArgs = new QuestionEventArgs(question);
        OnReceivedQuestion(questionEventArgs);
    }
}
