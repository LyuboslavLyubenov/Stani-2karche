using UnityEngine;
using System.IO;
using System;
using System.Collections.Generic;

public class GameData : MonoBehaviour
{
    const string LevelPath = "LevelData/";
    const int MarkMin = 3;
    const int MarkMax = 6;

    int currentQuestionIndex = 0;
    int currentMarkIndex = 0;

    List<List<Question>> marksQuestions = new List<List<Question>>();

    public EventHandler<MarkEventArgs> MarkIncrease = delegate
    {
    };

    void Start()
    {
        SerializeLevelData();
    }

    void SerializeLevelData()
    {
        for (int i = MarkMin; i <= MarkMax; i++)
        {
            var filePath = string.Format("{0}{1}.csv", LevelPath, i);
            var questionsSerialized = new List<Question>();

            using (StreamReader markQuestionsSR = new StreamReader(filePath, System.Text.Encoding.UTF8))
            {
                while (!markQuestionsSR.EndOfStream)
                {
                    var questionText = markQuestionsSR.ReadLine().Split(',')[0];
                    var answersText = new string[4];
                    var correctAnswerIndex = -1;

                    for (int j = 0; j < 4; j++)
                    {
                        var answerParams = markQuestionsSR.ReadLine().Split(',');
                        answersText[j] = answerParams[0];

                        if (answerParams[1].ToLower() == "верен")
                        {
                            correctAnswerIndex = j;
                        }
                    }

                    var question = new Question(questionText, answersText, correctAnswerIndex);
                    questionsSerialized.Add(question);
                    markQuestionsSR.ReadLine();
                }
            }

            marksQuestions.Add(questionsSerialized);
        }
    }

    public Question GetCurrentQuestion()
    {
        var questions = marksQuestions[currentMarkIndex];
        var index = Mathf.Min(questions.Count - 1, currentQuestionIndex - 1);
        return questions[index];
    }

    public Question GetNextQuestion()
    {
        var questions = marksQuestions[currentMarkIndex];

        if (currentQuestionIndex >= questions.Count)
        {
            if (currentMarkIndex >= marksQuestions.Count)
            {
                return null;
            }
            else
            {
                currentMarkIndex++;
                currentQuestionIndex = 0;

                MarkIncrease(this, new MarkEventArgs(currentMarkIndex + 2));
            }   
        }

        var question = questions[currentQuestionIndex++];
        return question;
    }
}

