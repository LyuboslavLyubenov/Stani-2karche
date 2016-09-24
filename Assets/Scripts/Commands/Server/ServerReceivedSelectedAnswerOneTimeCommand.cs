using System.Collections.Generic;
using System;
using UnityEngine;

public class ReceivedServerAnswerSelectedCommand : INetworkManagerCommand
{
    public delegate void OnReceivedAnswerDelegate(int clientId,string answer);

    OnReceivedAnswerDelegate onReceivedAnswer;

    public ReceivedServerAnswerSelectedCommand(OnReceivedAnswerDelegate onReceivedAnswer)
    {
        if (onReceivedAnswer == null)
        {
            throw new ArgumentNullException("onReceivedAnswer");
        }

        this.onReceivedAnswer = onReceivedAnswer;
    }

    public virtual void Execute(Dictionary<string, string> commandsOptionsValues)
    {
        var connectionId = int.Parse(commandsOptionsValues["ConnectionId"]);
        var answerSelected = commandsOptionsValues["Answer"];

        onReceivedAnswer(connectionId, answerSelected);
        //TODO:
    }
}

public class ServerReceivedSelectedAnswerOneTimeCommand : ReceivedServerAnswerSelectedCommand, IOneTimeExecuteCommand
{
    public bool FinishedExecution
    {
        get;
        private set;
    }

    public EventHandler OnFinishedExecution
    {
        get;
        set;
    }

    public ServerReceivedSelectedAnswerOneTimeCommand(OnReceivedAnswerDelegate onReceivedAnswer)
        : base(onReceivedAnswer)
    {
        FinishedExecution = false;
    }

    public override void Execute(Dictionary<string, string> commandsOptionsValues)
    {
        base.Execute(commandsOptionsValues);
        FinishedExecution = true;

        if (OnFinishedExecution != null)
        {
            OnFinishedExecution(this, EventArgs.Empty);
        }
    }
}