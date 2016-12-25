using System;
using System.Collections.Generic;

using UnityEngine;

namespace Assets.Scripts.Commands.GameData
{

    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Network;

    public class GameDataQuestionCommand : INetworkManagerCommand
    {
        public delegate void ReceivedQuestionDelegate(QuestionRequestType requestType,ISimpleQuestion question,int questionsRemainingToNextMark,int secondsForAnswerQuestion);

        ReceivedQuestionDelegate receivedQuestion;

        public GameDataQuestionCommand(ReceivedQuestionDelegate receivedQuestion)
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
            var question = JsonUtility.FromJson<SimpleQuestion_Serializable>(questionJSON);
            var questionsRemaining = int.Parse(commandsOptionsValues["RemainingQuestionsToNextMark"]);
            var secondsForAnswerQuestion = int.Parse(commandsOptionsValues["SecondsForAnswerQuestion"]);
            var requestType = (QuestionRequestType)Enum.Parse(typeof(QuestionRequestType), commandsOptionsValues["RequestType"]);

            this.receivedQuestion(requestType, question.Deserialize(), questionsRemaining, secondsForAnswerQuestion);
        }
    }

}