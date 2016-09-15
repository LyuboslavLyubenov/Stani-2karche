using System;
using System.Collections.Generic;
using UnityEngine;

public class LoadQuestionCommand : INetworkManagerCommand
{
    Action<Question> OnReceivedQuestion;

    public LoadQuestionCommand(Action<Question> OnReceivedQuestion)
    {
        if (OnReceivedQuestion == null)
        {
            throw new ArgumentNullException("OnReceivedQuestion");
        }
            
        this.OnReceivedQuestion = OnReceivedQuestion;
    }

    public void Execute(Dictionary<string, string> commandsOptionsValues)
    {
        var questionJSON = commandsOptionsValues["QuestionJSON"];
        var question = JsonUtility.FromJson<Question>(questionJSON);
        OnReceivedQuestion(question);
    }
}
