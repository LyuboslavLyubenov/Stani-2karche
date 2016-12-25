using System;
using System.Collections.Generic;

using UnityEngine;

namespace Assets.Scripts.Commands.Client
{

    using Assets.Scripts.Interfaces;

    public class LoadQuestionCommand : INetworkManagerCommand
    {
        public delegate void OnReceivedQuestion(ISimpleQuestion question,int timeToAnswer);

        OnReceivedQuestion onReceivedQuestion;

        public LoadQuestionCommand(OnReceivedQuestion onReceivedQuestion)
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
            this.onReceivedQuestion(question.Deserialize(), timeToAnswer);
        }
    }

}
