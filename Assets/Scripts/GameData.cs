using UnityEngine;
using System.IO;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using CielaSpike;
using System.Threading;

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

    public List<int> QuestionsToTakePerMark
    {
        get
        {
            return questionsToTakePerMark;
        }
    }

    public int CurrentMarkIndex
    {
        get
        {
            return currentMarkIndex;
        }
    }

    bool loaded = false;

    int currentQuestionIndex = 0;
    int currentMarkIndex = 0;

    List<List<Question>> marksQuestions = new List<List<Question>>();
    List<int> questionsToTakePerMark = new List<int>();

    void Start()
    {
        this.StartCoroutineAsync(SerializeLevelDataAsync());
    }

    IEnumerator SerializeLevelDataAsync()
    {
        Thread.Sleep(33);
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

            using (FileStream fs = new FileStream(filePath, FileMode.Open))
            {
                using (StreamReader markQuestionsSR = new StreamReader(fs, System.Text.Encoding.Default, true))
                {
                    var questionSettings = markQuestionsSR.ReadLine().Split(',');

                    if (questionSettings.Length < 2)
                    {
                        questionsToAdd = int.MaxValue;    
                    }
                    else
                    {
                        questionsToAdd = int.Parse(questionSettings[1]);    
                    }

                    questionsToTakePerMark.Add(questionsToAdd);

                    markQuestionsSR.ReadLine();

                    while (!markQuestionsSR.EndOfStream)
                    {
                        var questionText = markQuestionsSR.ReadLine().Trim(new char[] { '\"', ',' });
                        var answersText = new List<string>();
                        var correctAnswerIndex = -1;
                        var correctAnswer = "";

                        for (int j = 0; j < 4; j++)
                        {
                            var answerParams = markQuestionsSR.ReadLine().Split(',');
                            var answerTextNotFiltered = answerParams.Take(answerParams.Length - 1).ToArray();
                            var answerText = string.Join("", answerTextNotFiltered).Trim(new char[] { '\"', ',' });
                            var isAnswerCorrect = answerParams.Last().ToLower() == "верен";

                            answersText.Add(answerText);

                            if (isAnswerCorrect)
                            {
                                correctAnswer = answerText;
                            }
                        }

                        if (ShuffleAnswers)
                        {
                            answersText.Shuffle();
                        }

                        correctAnswerIndex = answersText.IndexOf(correctAnswer);

                        if (correctAnswerIndex == -1)
                        {
                            throw new Exception("Въпрос номер" + questionsSerialized.Count + " във файл " + i + ".csv има само грешни отговори.");
                            //question cannot have only wrong answers
                        }

                        var question = new Question(questionText, answersText.ToArray(), correctAnswerIndex);
                        questionsSerialized.Add(question);
                        markQuestionsSR.ReadLine();
                    }
                }                
            }

            if (ShuffleQuestions)
            {
                questionsSerialized.Shuffle();
            }

            var questions = questionsSerialized.ToList();
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

        if (currentMarkIndex >= marksQuestions.Count)
        {
            return null;
        }

        var questions = marksQuestions[currentMarkIndex];
        var index = Mathf.Min(questions.Count - 1, currentQuestionIndex);
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

        if (currentMarkIndex >= marksQuestions.Count)
        {
            return null;
        }

        var nextQuestionIndex = currentQuestionIndex + 1;
        var questions = marksQuestions[currentMarkIndex];
        var questionsToTake = questionsToTakePerMark[currentMarkIndex];

        if (questionsToTake > questions.Count)
        {
            questionsToTake = questions.Count;
        }

        if (nextQuestionIndex >= questionsToTake)
        {
            currentMarkIndex++;
            currentQuestionIndex = 0;
            MarkIncrease(this, new MarkEventArgs(currentMarkIndex + 2));

            return GetCurrentQuestion();
        }
        else
        {
            currentQuestionIndex = nextQuestionIndex;

            var question = questions[currentQuestionIndex];
            return question;
        }
    }
}

