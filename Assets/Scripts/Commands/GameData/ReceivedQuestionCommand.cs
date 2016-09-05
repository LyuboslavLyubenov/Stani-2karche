using System;
using System.Collections.Generic;
using UnityEngine;


public class ReceivedQuestionCommand : INetworkManagerCommand
{
    public delegate void ReceivedQuestionDelegate(QuestionRequestType requestType,Question question,int questionsRemainingToNextMark);

    ReceivedQuestionDelegate receivedQuestion;

    public ReceivedQuestionCommand(ReceivedQuestionDelegate receivedQuestion)
    {
        if (receivedQuestion == null)
        {
            throw new ArgumentNullException("receivedQuestion");
        }

        this.receivedQuestion = receivedQuestion;
    }

    public void Execute(Dictionary<string, string> commandsOptionsValues)
    {
        var questionJSON = commandsOptionsValues["QuestionJSON"];
        var question = JsonUtility.FromJson<Question>(questionJSON);
        var questionsRemaining = int.Parse(commandsOptionsValues["RemainingQuestionsToNextMark"]);
        var requestType = (QuestionRequestType)Enum.Parse(typeof(QuestionRequestType), commandsOptionsValues["RequestType"]);

        receivedQuestion(requestType, question, questionsRemaining);
    }
}