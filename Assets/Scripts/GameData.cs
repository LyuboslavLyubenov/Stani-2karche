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
                    var questionSettings = markQuestionsSR.ReadLine().Trim('\"').Split(',');

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
                        var questionText = markQuestionsSR.ReadLine().Trim('\"').Split(',')[0];
                        var answersText = new List<string>();
                        var correctAnswerIndex = -1;
                        var correctAnswer = "";

                        for (int j = 0; j < 4; j++)
                        {
                            var answerParams = markQuestionsSR.ReadLine().Trim('\"').Split(',');
                            answersText.Add(answerParams[0]);

                            if (answerParams.Length == 2 && answerParams[1].ToLower() == "верен")
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
                            throw new Exception("Въпрос номер " + questionsSerialized.Count + " има само грешни отговори.");
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

        var questions = marksQuestions[currentMarkIndex];
        var questionsToTake = questionsToTakePerMark[currentMarkIndex];

        if (questionsToTake > questions.Count)
        {
            questionsToTake = questions.Count;
        }

        if (currentQuestionIndex >= questionsToTake)
        {
            if (currentMarkIndex >= marksQuestions.Count)
            {
                return null;
            }
            else
            {
                currentMarkIndex++;
                currentQuestionIndex = 0;

                questions = marksQuestions[currentMarkIndex];

                MarkIncrease(this, new MarkEventArgs(currentMarkIndex + 2));
            }   
        }

        var question = questions[currentQuestionIndex++];
        return question;
    }
}

