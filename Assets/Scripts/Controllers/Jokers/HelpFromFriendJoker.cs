using System.Linq;
using System;
using UnityEngine;

public class HelpFromFriendJoker : IJoker
{
    //0% = 0f, 100% = 1f
    const float ChanceForGeneratingCorrectAnswer = 0.85f;

    public EventHandler<AnswerEventArgs> OnFriendAnswerGenerated = delegate
    {
    };

    public EventHandler ShowOnlineCallFriendMenu = delegate
    {
    };

    IGameData gameData;
    ClientNetworkManager networkManager;

    public Sprite Image
    {
        get;
        private set;
    }

    EventHandler OnSuccessfullyActivated
    {
        get;
        set;
    }

    public HelpFromFriendJoker(IGameData gameData, ClientNetworkManager networkManager)
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

        //Image = Resources.Load()
    }

    public void Activate()
    {
        gameData.GetCurrentQuestion(ActivateCallAFriendJoker, DebugUtils.LogException);
    }

    void ActivateCallAFriendJoker(Question currentQuestion)
    {
        if (networkManager.ServerConnectedClientsCount <= 1)
        {
            //generate question
            var answers = currentQuestion.Answers;
            var correctAnswer = answers[currentQuestion.CorrectAnswerIndex];
            var answerSelected = answers[currentQuestion.CorrectAnswerIndex];
            var isCorrect = true;

            if (UnityEngine.Random.value >= ChanceForGeneratingCorrectAnswer)
            {
                var wrongAnswers = answers.Where(a => a != correctAnswer).ToArray();
                var wrongAnswerIndex = UnityEngine.Random.Range(0, wrongAnswers.Length);

                answerSelected = wrongAnswers[wrongAnswerIndex];
                isCorrect = false;
            }

            var answerEventArgs = new AnswerEventArgs(answerSelected, isCorrect);
            OnFriendAnswerGenerated(this, answerEventArgs);
        }
        else
        {
            ShowOnlineCallFriendMenu(this, EventArgs.Empty); 
        }

        if (OnSuccessfullyActivated != null)
        {
            OnSuccessfullyActivated(this, EventArgs.Empty); 
        }
    }
}
