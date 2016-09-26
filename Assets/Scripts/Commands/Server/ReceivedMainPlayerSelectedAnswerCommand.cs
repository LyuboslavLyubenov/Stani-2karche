﻿using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

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
        onReceivedClickedAnswer(answerEventArgs);
    }

}