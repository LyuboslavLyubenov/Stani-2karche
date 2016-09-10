using UnityEngine;
using System.IO;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using CSharpJExcel.Jxl;
using CielaSpike;
using System.Threading;

public class LocalGameData : MonoBehaviour, IGameData
{
    const string LevelPath = "LevelData\\теми\\";
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

    public string LevelCategory = "философия";

    public bool Loaded
    {
        get
        {
            return loaded; 
        }
    }

    public EventHandler<MarkEventArgs> OnMarkIncrease
    {
        get;
        set;
    }

    public int RemainingQuestionsToNextMark
    {
        get
        {
            return questionsToTakePerMark[currentMarkIndex] - currentQuestionIndex - 1;    
        }
    }

    public bool IsLastQuestion
    {
        get
        {
            return (currentMarkIndex < marksQuestions.Count) && ((currentQuestionIndex + 1) >= marksQuestions[currentMarkIndex].Count);
        }
    }

    public int CurrentMark
    {
        get
        {
            return currentMarkIndex + (MarkMin - 1);
        }
    }

    bool loaded = false;

    int currentQuestionIndex = 0;
    int currentMarkIndex = 0;

    List<List<Question>> marksQuestions = new List<List<Question>>();
    List<int> questionsToTakePerMark = new List<int>();

    IEnumerator SerializeLevelDataAsync()
    {
        Thread.Sleep(100);
        SerializeLevelData();
        loaded = true;
        yield return null;
    }

    /// <summary>
    /// Load all questions and seperate them by categories
    /// </summary>
    void SerializeLevelData()
    {
        var levelPath = Directory.GetCurrentDirectory() + '\\' + LevelPath + LevelCategory;
        var questionFilesPath = Directory.GetFiles(levelPath).Where(p => p.EndsWith(".xls")).ToArray();

        for (int i = 0; i < questionFilesPath.Length; i++)
        {
            var questions = new List<Question>();
            var markQuestionsDataPath = questionFilesPath[i];
            var workbook = Workbook.getWorkbook(new FileInfo(markQuestionsDataPath));
            var sheet = workbook.getSheet(0);
            var questionsToTake = sheet.getCell(1, 0).getContents();

            questionsToTakePerMark.Add(int.Parse(questionsToTake));

            for (int rowi = 2; rowi < sheet.getRows() - 6; rowi += 6)
            {
                var questionText = sheet.getCell(0, rowi).getContents();

                if (string.IsNullOrEmpty(questionText))
                {
                    throw new Exception("Празен въпрос. Във файл " + (MarkMin + i) + ".xls на ред " + (rowi + 1));    
                }

                var answers = new List<string>();
                var correctAnswer = "";

                for (int answersRowI = rowi + 1; answersRowI < rowi + 5; answersRowI++)
                {   
                    var answerText = sheet.getCell(0, answersRowI).getContents();
                    var isCorrect = sheet.getCell(1, answersRowI).getContents().ToLower() == "верен";

                    if (string.IsNullOrEmpty(answerText))
                    {
                        throw new Exception("Не може да има празен отговор. Файл " + (MarkMin + i) + ".xls на ред " + (answersRowI + 1));
                    }

                    answers.Add(answerText);

                    if (isCorrect)
                    {
                        if (!string.IsNullOrEmpty(correctAnswer))
                        {
                            throw new Exception("Не може да има 2 верни отговора на 1 въпрос. Файл " + (MarkMin + i) + ".xls на ред " + (answersRowI + 1));    
                        }

                        correctAnswer = answerText;
                    }
                }

                if (string.IsNullOrEmpty(correctAnswer))
                {
                    throw new Exception("Няма правилен отговор. Файл " + MarkMin + i + ".xls на въпрос на ред " + rowi + 1);
                }

                if (ShuffleAnswers)
                {
                    answers.Shuffle();
                }

                var correctAnswerIndex = answers.IndexOf(correctAnswer);
                var question = new Question(questionText, answers.ToArray(), correctAnswerIndex);

                questions.Add(question);
            }

            if (ShuffleQuestions)
            {
                questions.Shuffle();
            }

            marksQuestions.Add(questions);
        }
    }

    public void LoadDataAsync()
    {
        this.StartCoroutineAsync(SerializeLevelDataAsync());
    }

    Question _GetCurrentQuestion()
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

    Question _GetNextQuestion()
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

            if (OnMarkIncrease != null)
            {
                OnMarkIncrease(this, new MarkEventArgs(currentMarkIndex + 2));
            }

            return _GetCurrentQuestion();
        }
        else
        {
            currentQuestionIndex = nextQuestionIndex;

            var question = questions[currentQuestionIndex];
            return question;
        }
    }

    Question _GetRandomQuestion()
    {
        var questionIndex = UnityEngine.Random.Range(0, marksQuestions[currentMarkIndex].Count);
        return marksQuestions[currentMarkIndex][questionIndex];
    }

    public void GetCurrentQuestion(Action<Question> onSuccessfullyLoaded, Action<Exception> onError = null)
    {
        try
        {
            var question = _GetCurrentQuestion();
            onSuccessfullyLoaded(question);
        }
        catch (Exception ex)
        {
            onError(ex);
        }
    }

    public void GetNextQuestion(Action<Question> onSuccessfullyLoaded, Action<Exception> onError = null)
    {        
        try
        {
            var question = _GetNextQuestion();
            onSuccessfullyLoaded(question);
        }
        catch (Exception ex)
        {
            onError(ex);
        }
    }

    public void GetRandomQuestion(Action<Question> onSuccessfullyLoaded, Action<Exception> onError = null)
    {
        try
        {
            var question = _GetRandomQuestion();
            onSuccessfullyLoaded(question);
        }
        catch (Exception ex)
        {
            onError(ex);
        }
    }
}

