using UnityEngine;
using System.IO;
using System;
using System.Collections.Generic;
using System.Collections;
using CielaSpike;
using System.Linq;

public class GameData : MonoBehaviour
{
    const string LevelPath = "LevelData/";
    const int MarkMin = 3;
    const int MarkMax = 6;

    /// <summary>
    /// If true questions for given marks are aways with randomized order
    /// </summary>
    public bool ShuffleQuestions = true;
    /// <summary>
    /// If true answers for every questions will be in random arrangement
    /// </summary>
    public bool ShuffleAnswers = true;

    public EventHandler<MarkEventArgs> MarkIncrease = delegate
    {
    };

    public bool Loaded
    {
        get
        {
            return loaded; 
        }
    }

    bool loaded = false;

    int nextQuestionIndex = 0;
    int currentMarkIndex = 0;

    List<List<Question>> marksQuestions = new List<List<Question>>();

    void Start()
    {
        this.StartCoroutineAsync(SerializeLevelDataAsync());
    }

    IEnumerator SerializeLevelDataAsync()
    {
        yield return Ninja.JumpToUnity;
        marksQuestions = SerializeLevelData();
        loaded = true;
        yield return null;
    }

    /// <summary>
    /// Load all questions and seperate them by categories
    /// </summary>
    List<List<Question>> SerializeLevelData()
    {
        var serializedMarksQuestions = new List<List<Question>>();

        for (int i = MarkMin; i <= MarkMax; i++)
        {
            //get file path for the current mark
            var filePath = string.Format("{0}{1}.csv", LevelPath, i);
            var questionsSerialized = new List<Question>();
            var questionsToAdd = 0;

            using (StreamReader markQuestionsSR = new StreamReader(filePath, System.Text.Encoding.Default, true))
            {
                var questionSettings = markQuestionsSR.ReadLine().Replace("\"", "").Split(',');
                questionsToAdd = int.Parse(questionSettings[1]);

                markQuestionsSR.ReadLine();

                while (!markQuestionsSR.EndOfStream)
                {
                    var questionText = markQuestionsSR.ReadLine().Replace("\"", "").Split(',')[0];
                    var answersText = new List<string>();
                    var correctAnswerIndex = -1;
                    var correctAnswer = "";

                    for (int j = 0; j < 4; j++)
                    {
                        var answerParams = markQuestionsSR.ReadLine().Replace("\"", "").Split(',');
                        answersText.Add(answerParams[0]);

                        if (answerParams[1].ToLower() == "верен")
                        {
                            correctAnswer = answerParams[0];
                        }
                    }

                    if (ShuffleAnswers)
                    {
                        answersText.Shuffle();
                    }

                    correctAnswerIndex = answersText.IndexOf(correctAnswer);

                    if (correctAnswerIndex == -1)
                    {
                        //question cannot have only wrong answers
                        continue;
                    }

                    var question = new Question(questionText, answersText.ToArray(), correctAnswerIndex);
                    questionsSerialized.Add(question);
                    markQuestionsSR.ReadLine();
                }
            }


            if (ShuffleQuestions)
            {
                questionsSerialized.Shuffle();
            }

            var questions = questionsSerialized.Take(questionsToAdd).ToList();
            serializedMarksQuestions.Add(questions);
        }

        return serializedMarksQuestions;
    }

    /// <summary>
    /// Gets the current question.
    /// </summary>
    /// <returns>The current question.</returns>
    public Question GetCurrentQuestion()
    {
        if (!loaded)
        {
            throw new Exception("Not loaded questions yet");
        }

        var questions = marksQuestions[currentMarkIndex];
        var index = Mathf.Min(questions.Count - 1, nextQuestionIndex - 1);
        return questions[index];
    }

    /// <summary>
    /// Gets the next question.
    /// </summary>
    /// <returns>The next question.</returns>
    public Question GetNextQuestion()
    {
        if (!loaded)
        {
            throw new Exception("Not loaded questions yet");
        }

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

