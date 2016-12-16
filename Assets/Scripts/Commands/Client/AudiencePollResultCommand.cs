using UnityEngine;
using System;
using System.Collections.Generic;
using System.Timers;
using System.Linq;

public class AudiencePollResultCommand : IOneTimeExecuteCommand
{
    public EventHandler OnFinishedExecution
    {
        get;
        set;
    }

    public bool FinishedExecution
    {
        get;
        private set;
    }

    Action<Dictionary<string, int>> onReceivedAnswersVotes;

    public AudiencePollResultCommand(Action<Dictionary<string, int>> onReceivedAnswersVotes)
    {
        if (onReceivedAnswersVotes == null)
        {
            throw new ArgumentNullException("onReceivedAnswersVotes");
        }
            
        this.onReceivedAnswersVotes = onReceivedAnswersVotes;
    }

    public void Execute(Dictionary<string, string> commandsOptionsValues)
    {
        var answersVotes = commandsOptionsValues.Where(optionValue => optionValue.Key != "ConnectionId")
            .ToArray();

        var answersVotesData = new Dictionary<string, int>();

        for (int i = 0; i < answersVotes.Length; i++)
        {
            var answer = answersVotes[i].Key;
            var voteCount = int.Parse(answersVotes[i].Value);
            answersVotesData.Add(answer, voteCount);
        }

        onReceivedAnswersVotes(answersVotesData);

        FinishedExecution = true;

        if (OnFinishedExecution != null)
        {
            OnFinishedExecution(this, EventArgs.Empty);
        }
    }
}