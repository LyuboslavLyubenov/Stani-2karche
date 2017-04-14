namespace Commands.Server
{

    using System;
    using System.Collections.Generic;

    using EventArgs;

    using Interfaces.Network.NetworkManager;

    using UnityEngine;

    public class MainPlayerSelectedAnswerCommand : INetworkManagerCommand
    {
        private Action<AnswerEventArgs_Serializable> onReceivedClickedAnswer;

        public MainPlayerSelectedAnswerCommand(Action<AnswerEventArgs_Serializable> onReceivedClickedAnswer)
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
