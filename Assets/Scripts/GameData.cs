using UnityEngine;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;

public class GameData : MonoBehaviour
{
    const string LevelPath = "LevelData/";

    /// <summary>
    /// If true questions for given marks are aways with randomized order
    /// </summary>
    public bool ShouldShuffleQuestions = true;
    /// <summary>
    /// If true answers for every questions will be in random arrangement
    /// </summary>
    public bool ShouldShuffleAnswers = true;

    const int MarkMin = 3;
    const int MarkMax = 6;

    int nextQuestionIndex = 0;
    int currentMarkIndex = 0;

    List<List<Question>> marksQuestions = new List<List<Question>>();

    public EventHandler<MarkEventArgs> MarkIncrease = delegate
    {
    };

    void Start()
    {
        marksQuestions = SerializeLevelData();
    }

    List<List<Question>> SerializeLevelData()
    {
        var serializedMarksQuestions = new List<List<Question>>();

        for (int i = MarkMin; i <= MarkMax; i++)
        {
            var filePath = string.Format("{0}{1}.csv", LevelPath, i);
            var questionsSerialized = new List<Question>();

            using (StreamReader markQuestionsSR = new StreamReader(filePath, System.Text.Encoding.UTF8))
            {
                while (!markQuestionsSR.EndOfStream)
                {
                    var questionText = markQuestionsSR.ReadLine().Split(',')[0];
                    var answersText = new List<string>();
                    var correctAnswerIndex = -1;
                    var correctAnswer = "";

                    for (int j = 0; j < 4; j++)
                    {
                        var answerParams = markQuestionsSR.ReadLine().Split(',');
                        answersText.Add(answerParams[0]);

                        if (answerParams[1].ToLower() == "верен")
                        {
                            correctAnswer = answerParams[0];
                        }
                    }

                    if (ShouldShuffleAnswers)
                    {
                        answersText.Shuffle();
                    }

                    correctAnswerIndex = answersText.IndexOf(correctAnswer);

                    var question = new Question(questionText, answersText.ToArray(), correctAnswerIndex);
                    questionsSerialized.Add(question);
                    markQuestionsSR.ReadLine();
                }
            }

            if (ShouldShuffleQuestions)
            {
                questionsSerialized.Shuffle();
            }

            serializedMarksQuestions.Add(questionsSerialized);
        }

        return serializedMarksQuestions;
    }

    public Question GetCurrentQuestion()
    {
        var questions = marksQuestions[currentMarkIndex];
        var index = Mathf.Min(questions.Count - 1, nextQuestionIndex - 1);
        return questions[index];
    }

    public Question GetNextQuestion()
    {
        var questions = marksQuestions[currentMarkIndex];

        if (nextQuestionIndex >= questions.Count)
        {
            if (currentMarkIndex >= marksQuestions.Count)
            {
                return null;
            }
            else
            {
                currentMarkIndex++;
                nextQuestionIndex = 0;

                MarkIncrease(this, new MarkEventArgs(currentMarkIndex + 2));
            }   
        }

        var question = questions[nextQuestionIndex++];
        return question;
    }
}

