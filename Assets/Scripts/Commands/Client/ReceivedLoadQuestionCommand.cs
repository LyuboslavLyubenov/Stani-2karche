using System;
using System.Collections.Generic;
using UnityEngine;

public class ReceivedLoadQuestionCommand : INetworkManagerCommand
{
    Action<ISimpleQuestion> OnReceivedQuestion;

    public ReceivedLoadQuestionCommand(Action<ISimpleQuestion> OnReceivedQuestion)
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
        var question = JsonUtility.FromJson<SimpleQuestion_Serializable>(questionJSON);
        OnReceivedQuestion(question.Deserialize());
    }
}
