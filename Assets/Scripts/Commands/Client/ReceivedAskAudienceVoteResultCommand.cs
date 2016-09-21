using UnityEngine;
using System;
using System.Collections.Generic;
using System.Timers;
using System.Linq;

public class ReceivedAskAudienceVoteResultCommand : IOneTimeExecuteCommand
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

    AudienceAnswerUIController audienceAnswerUIController;

    GameObject audienceAnswerUI;

    public ReceivedAskAudienceVoteResultCommand(GameObject audienceAnswerUI)
    {
        if (audienceAnswerUI == null)
        {
            throw new ArgumentNullException("audienceAnswerUI");
        }
         
        this.audienceAnswerUI = audienceAnswerUI;
        this.audienceAnswerUIController = audienceAnswerUI.GetComponent<AudienceAnswerUIController>();
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

        audienceAnswerUI.SetActive(true);
        audienceAnswerUIController.SetVoteCount(answersVotesData, true);

        FinishedExecution = true;

        if (OnFinishedExecution != null)
        {
            OnFinishedExecution(this, EventArgs.Empty);
        }
    }
}