using System;
using System.Collections.Generic;

using UnityEngine;

namespace Assets.Scripts.Commands.Server
{

    using Assets.Scripts.EventArgs;
    using Assets.Scripts.Interfaces;

    public class ReceivedMainPlayerSelectedAnswerCommand : INetworkManagerCommand
    {
        Action<AnswerEventArgs_Serializable> onReceivedClickedAnswer;

        public ReceivedMainPlayerSelectedAnswerCommand(Action<AnswerEventArgs_Serializable> onReceivedClickedAnswer)
        {
            if (onReceivedClickedAnswer == null)
            {
                throw new ArgumentNullException("onReceivedClickedAnswer");
            }
            
            this.onReceivedClickedAnswer = onReceivedClickedAnswer;
        }

        public void Execute(Dictionary<string, string> commandsOptionsValues)
        {
            var answerEventArgsJSON = commandsOptionsValues["AnswerEventArgsJSON"];
            var answerEventArgs = JsonUtility.FromJson<AnswerEventArgs_Serializable>(answerEventArgsJSON);
            this.onReceivedClickedAnswer(answerEventArgs);
        }

    }

}
