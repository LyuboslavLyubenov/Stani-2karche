using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class FifthyFifthyChanceJoker : IJoker
{
    QuestionUIController questionUIController;
    IGameData gameData;

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

    public FifthyFifthyChanceJoker(IGameData gameData, QuestionUIController questionUIController)
    {
        if (gameData == null)
        {
            throw new ArgumentNullException("gameData");
        }
            
        if (questionUIController == null)
        {
            throw new ArgumentNullException("questionUIController");
        }
            
        this.questionUIController = questionUIController;
        this.gameData = gameData;

        Image = Resources.Load<Sprite>("Images/Buttons/Jokers/FifthyFifthyChance");
    }

    public void Activate()
    {
        gameData.GetCurrentQuestion(ActivateFifthyChanceJoker, DebugUtils.LogException);
    }

    void ActivateFifthyChanceJoker(Question currentQuestion)
    {
        var correctAnswerIndex = currentQuestion.CorrectAnswerIndex;
        var disabledAnswersIndex = new List<int>();

        for (int i = 0; i < 2; i++)
        {
            int n;

            while (true)
            {
                //make sure we dont disable correct answer and we dont disable answer 2 times
                n = UnityEngine.Random.Range(0, 4);

                if (n != correctAnswerIndex && !disabledAnswersIndex.Contains(n))
                {
                    break;
                }
            }

            disabledAnswersIndex.Add(n);
        }

        for (int i = 0; i < disabledAnswersIndex.Count; i++)
        {
            var disabledIndex = disabledAnswersIndex[i];
            questionUIController.HideAnswer(disabledIndex);
        }
    }
}
