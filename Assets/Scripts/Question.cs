using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class Question
{
    public string Text;
    public string[] Answers;
    public int CorrectAnswerIndex;

    public Question(string text, string[] answers, int correctAnswerIndex)
    {
        if (string.IsNullOrEmpty(text))
        {
            throw new ArgumentException("Въпросът не трябва да е празен");
        }


        if (answers.Length != 4)
        {
            throw new ArgumentException("Отговорите трябва да са 4 на брои");
        }

        for (int i = 0; i < answers.Length; i++)
        {
            if (string.IsNullOrEmpty(answers[i]))
            {
                throw new ArgumentException("Отговор номер " + i + "е празен. Невъзможно е да има празен отговор.");
            }
        }
            
        if (correctAnswerIndex < 0 || correctAnswerIndex > 3)
        {
            throw new ArgumentOutOfRangeException("correctAnswerIndex", "correctAnswerIndex трябва да бъде със стойности между 0 и 3 (включително)");
        }


        this.Text = text;
        this.Answers = answers;
        this.CorrectAnswerIndex = correctAnswerIndex;
    }
}
