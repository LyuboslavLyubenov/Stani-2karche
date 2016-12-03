using System;
using System.Collections.Generic;
using UnityEngine;

public class ReceivedLoadQuestionCommand : INetworkManagerCommand
{
    public delegate void OnReceivedQuestion(ISimpleQuestion question,int timeToAnswer);

    OnReceivedQuestion onReceivedQuestion;

    public ReceivedLoadQuestionCommand(OnReceivedQuestion onReceivedQuestion)
    {
        if (onReceivedQuestion == null)
        {
            throw new ArgumentNullException("onReceivedQuestion");
        }
            
        this.onReceivedQuestion = onReceivedQuestion;
    }

    public void Execute(Dictionary<string, string> commandsOptionsValues)
    {
        var questionJSON = commandsOptionsValues["QuestionJSON"];
        var timeToAnswer = int.Parse(commandsOptionsValues["TimeToAnswer"]);
        var question = JsonUtility.FromJson<SimpleQuestion_Serializable>(questionJSON);
        onReceivedQuestion(question.Deserialize(), timeToAnswer);
    }
}
