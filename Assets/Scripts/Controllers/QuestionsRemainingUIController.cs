using UnityEngine;
using UnityEngine.UI;
using System;

public class QuestionsRemainingUIController : MonoBehaviour
{
    public Text QuestionsRemaining;

    public void SetRemainingQuestions(int remainingQuestions)
    {
        if (remainingQuestions < 0)
        {
            throw new ArgumentOutOfRangeException("remainingQuestions");
        }

        QuestionsRemaining.text = remainingQuestions.ToString();
    }
}