using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class AskAudienceJoker : IJoker
{
    const int MinCorrectAnswerChance = 40;
    const int MaxCorrectAnswerChance = 85;

    const int MinClientsForOnlineVote_Relaese = 4;
    const int MinClientsForOnlineVote_Development = 1;

    public EventHandler<AudienceVoteEventArgs> OnAudienceVoteGenerated = delegate
    {
    };

    public EventHandler GetOnlineAudienceAnswer = delegate
    {
    };

    public Sprite Image
    {
        get;
        private set;
    }

    public EventHandler OnSuccessfullyActivated
    {
        get;
        set;
    }

    IGameData gameData;

    ClientNetworkManager networkManager;

    public AskAudienceJoker(IGameData gameData, ClientNetworkManager networkManager)
    {
        if (gameData == null)
        {
            throw new ArgumentNullException("gameData");
        }

        if (networkManager == null)
        {
            throw new ArgumentNullException("networkManager");
        }
            
        this.gameData = gameData;
        this.networkManager = networkManager;

        Image = Resources.Load<Sprite>("Images/Buttons/Jokers/AskAudience");
    }

    public void Activate()
    {
        gameData.GetCurrentQuestion(ActivateAskAudienceJoker, DebugUtils.LogException);
    }

    void ActivateAskAudienceJoker(Question currentQuestion)
    {
        var minForOnlineVote = MinClientsForOnlineVote_Relaese;

        #if DEVELOPMENT_BUILD
        minForOnlineVote = MinClientsForOnlineVote_Development;
        #endif

        if (networkManager.ServerConnectedClientsCount < minForOnlineVote)
        {
            var generatedAudienceAnswersVotes = GenerateAudienceVotes(currentQuestion);
            var audienceVoteEventArgs = new AudienceVoteEventArgs(generatedAudienceAnswersVotes);
            OnAudienceVoteGenerated(this, audienceVoteEventArgs);
        }
        else
        {
            GetOnlineAudienceAnswer(this, EventArgs.Empty);
        }
    }

    Dictionary<string, int> GenerateAudienceVotes(Question question)
    {
        var generatedAudienceAnswersVotes = new Dictionary<string, int>();
        var correctAnswer = question.Answers[question.CorrectAnswerIndex];
        var correctAnswerChance = UnityEngine.Random.Range(MinCorrectAnswerChance, MaxCorrectAnswerChance);
        var wrongAnswersLeftOverChance = 100 - correctAnswerChance;

        generatedAudienceAnswersVotes.Add(correctAnswer, correctAnswerChance);

        for (int i = 0; i < question.Answers.Length; i++)
        {
            if (i == question.CorrectAnswerIndex)
            {
                continue;
            }

            var wrongAnswerChance = UnityEngine.Random.Range(0, wrongAnswersLeftOverChance);
            generatedAudienceAnswersVotes.Add(question.Answers[i], wrongAnswersLeftOverChance);
            wrongAnswersLeftOverChance -= wrongAnswerChance;
        }

        return generatedAudienceAnswersVotes;
    }

}
