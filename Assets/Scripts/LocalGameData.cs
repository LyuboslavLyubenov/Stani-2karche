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
    const string LevelPath = "\\Server\\LevelData\\теми\\";
    const int DefaultSecondsForAnswerQuestion = 60;
    const int DefaultQuestionToTakePerMark = int.MaxValue;
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
        get;
        private set;
    }

    public bool Loading
    {
        get;
        private set;
    }

    public EventHandler OnLoaded
    {
        get;
        set;
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
            if (!Loaded)
            {
                throw new Exception("Not loaded");
            }

            return questionsToTakePerMark[currentMarkIndex] - currentQuestionIndex - 1;    
        }
    }

    public bool IsLastQuestion
    {
        get
        {
            if (!Loaded)
            {
                throw new Exception("Not loaded");
            }

            return (currentMarkIndex < marksQuestions.Count) && ((currentQuestionIndex + 1) >= marksQuestions[currentMarkIndex].Length);
        }
    }

    public int CurrentMark
    {
        get
        {
            if (!Loaded)
            {
                throw new Exception("Not loaded");
            }
            
            return currentMarkIndex + (MarkMin - 1);
        }
    }

    public int SecondsForAnswerQuestion
    {
        get
        {
            if (!Loaded)
            {
                throw new Exception("Not loaded");
            }

            return secondsForAnswerQuestionPerMark[currentMarkIndex];
        }
    }

    int currentQuestionIndex = 0;
    int currentMarkIndex = 0;

    List<ISimpleQuestion[]> marksQuestions = new List<ISimpleQuestion[]>();
    List<int> questionsToTakePerMark = new List<int>();
    List<int> secondsForAnswerQuestionPerMark = new List<int>();

    void Awake()
    {
        OnLoaded = delegate
        {
        };

        OnMarkIncrease = delegate
        {
        };
    }

    IEnumerator ExtractLevelDataAsync(Action<Exception> onError)
    {
        Exception exception = null;

        try
        {
            Loaded = false;
            Loading = true;
            ExtractLevelData();
            Loaded = true;
            Loading = false;
        }
        catch (Exception ex)
        {
            exception = ex;
        }

        if (exception == null)
        {
            yield return Ninja.JumpToUnity;
            OnLoaded(this, EventArgs.Empty);    
        }
        else
        {
            onError(exception);
        }
    }

    /// <summary>
    /// Load all questions and seperate them by categories
    /// </summary>
    void ExtractLevelData()
    {
        var levelPath = Directory.GetCurrentDirectory() + '\\' + LevelPath + LevelCategory;
        var questionFilesPath = Directory.GetFiles(levelPath).Where(p => p.EndsWith(".xls")).ToArray();

        for (int i = 0; i < questionFilesPath.Length; i++)
        {
            var markQuestionsDataPath = questionFilesPath[i];
            var workbook = Workbook.getWorkbook(new FileInfo(markQuestionsDataPath));
            var sheet = workbook.getSheet(0);
            int questionsToTake = DefaultQuestionToTakePerMark;
            int secondsForAnswerQuestion = DefaultSecondsForAnswerQuestion;
            ISimpleQuestion[] allQuestionsForMark;

            allQuestionsForMark = ExtractQuestionsFromWorksheet(sheet, i);    

            try
            {
                questionsToTake = int.Parse(sheet.getCell(1, 0).getContents());
            }
            catch (Exception ex)
            {
                var fileName = markQuestionsDataPath.Split('/').Last();
                var errorMsg = LanguagesManager.Instance.GetValue("Errors/CantExtractQuestionsToTakeCount");

                Debug.LogFormat(errorMsg, fileName);
            }

            try
            {
                secondsForAnswerQuestion = int.Parse(sheet.getCell(3, 0).getContents());    
            }
            catch (Exception ex)
            {
                var fileName = markQuestionsDataPath.Split('/').Last();
                var cantExtractSecondsForAnswerMsg = LanguagesManager.Instance.GetValue("Errors/CantExtractSecondsForAnswer");
                Debug.LogFormat(cantExtractSecondsForAnswerMsg, fileName, DefaultSecondsForAnswerQuestion);
            }

            secondsForAnswerQuestionPerMark.Add(secondsForAnswerQuestion);
            questionsToTakePerMark.Add(questionsToTake);

            if (ShuffleQuestions)
            {
                allQuestionsForMark.Shuffle();
            }

            marksQuestions.Add(allQuestionsForMark);
        }
    }

    ISimpleQuestion[] ExtractQuestionsFromWorksheet(Sheet sheet, int workbookMarkIndex)
    {
        var questions = new List<ISimpleQuestion>();

        for (int rowi = 2; rowi < sheet.getRows() - 6; rowi += 6)
        {
            var questionText = sheet.getCell(0, rowi).getContents();

            if (string.IsNullOrEmpty(questionText))
            {
                var fileName = (MarkMin + workbookMarkIndex) + ".xls";
                var rowNumber = (rowi + 1).ToString();
                var errorMsg = LanguagesManager.Instance.GetValue("Errors/EmptyQuestion");
                var errorFormatedMsg = string.Format(errorMsg, fileName, rowNumber);
                throw new Exception(errorFormatedMsg);    
            }

            var answers = new List<string>();
            var correctAnswer = "";

            for (int answersRowI = rowi + 1; answersRowI < rowi + 5; answersRowI++)
            {   
                var answerText = sheet.getCell(0, answersRowI).getContents();
                var isCorrect = sheet.getCell(1, answersRowI).getContents().ToUpperInvariant() == ("верен").ToUpperInvariant();

                if (string.IsNullOrEmpty(answerText))
                {                
                    var fileName = (MarkMin + workbookMarkIndex) + ".xls";
                    var rowNumber = (rowi + 1).ToString();
                    var errorMsg = LanguagesManager.Instance.GetValue("Errors/EmptyAnswer");
                    var errorFormatedMsg = string.Format(errorMsg, fileName, rowNumber);
                    throw new Exception(errorFormatedMsg);
                }

                answers.Add(answerText);

                if (isCorrect)
                {
                    if (!string.IsNullOrEmpty(correctAnswer))
                    {
                        var fileName = (MarkMin + workbookMarkIndex) + ".xls";
                        var questionNumber = questions.Count + 1;
                        var errorMsg = LanguagesManager.Instance.GetValue("Errors/MultipleCorrectAnswers");
                        var errorFormatedMsg = string.Format(errorMsg, fileName, questionNumber);
                        throw new Exception(errorFormatedMsg);    
                    }

                    correctAnswer = answerText;
                }
            }

            if (string.IsNullOrEmpty(correctAnswer))
            {
                var fileName = (MarkMin + workbookMarkIndex) + ".xls";
                var questionNumber = questions.Count + 1;
                var errorMsg = LanguagesManager.Instance.GetValue("Errors/NoCorrectAnswer");
                var errorFormatedMsg = string.Format(errorMsg, fileName, questionNumber);

                throw new Exception(errorFormatedMsg);
            }

            if (ShuffleAnswers)
            {
                answers.Shuffle();
            }

            var correctAnswerIndex = answers.IndexOf(correctAnswer);
            var question = new SimpleQuestion(questionText, answers.ToArray(), correctAnswerIndex);

            questions.Add(question);
        }

        return questions.ToArray();
    }

    public void LoadDataAsync(Action<Exception> onErrorLoading)
    {
        if (Loading)
        {
            var errorMsg = LanguagesManager.Instance.GetValue("Errors/CurrentlyLoadingQuestions");
            onErrorLoading(new InvalidOperationException(errorMsg));
            return;
        }

        this.StartCoroutineAsync(ExtractLevelDataAsync(onErrorLoading));
    }

    ISimpleQuestion _GetCurrentQuestion()
    {
        if (!Loaded)
        {
            var errorMsg = LanguagesManager.Instance.GetValue("Errors/NotLoadedQuestions");
            throw new Exception(errorMsg);
        }

        if (currentMarkIndex >= marksQuestions.Count)
        {
            return null;
        }

        var questions = marksQuestions[currentMarkIndex];
        var index = Mathf.Min(questions.Length - 1, currentQuestionIndex);
        return questions[index];
    }

    ISimpleQuestion _GetNextQuestion()
    {
        if (!Loaded)
        {
            var errorMsg = LanguagesManager.Instance.GetValue("Errors/NotLoadedQuestions");
            throw new Exception(errorMsg);
        }

        if (currentMarkIndex >= marksQuestions.Count)
        {
            return null;
        }

        var nextQuestionIndex = currentQuestionIndex + 1;
        var questions = marksQuestions[currentMarkIndex];
        var questionsToTake = questionsToTakePerMark[currentMarkIndex];

        if (questionsToTake > questions.Length)
        {
            questionsToTake = questions.Length;
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

    ISimpleQuestion _GetRandomQuestion()
    {
        if (!Loaded)
        {
            var errorMsg = LanguagesManager.Instance.GetValue("Errors/NotLoadedQuestions");
            throw new Exception(errorMsg);
        }

        var questionIndex = UnityEngine.Random.Range(0, marksQuestions[currentMarkIndex].Length);
        return marksQuestions[currentMarkIndex][questionIndex];
    }

    public void GetCurrentQuestion(Action<ISimpleQuestion> onSuccessfullyLoaded, Action<Exception> onError = null)
    {
        try
        {
            var question = _GetCurrentQuestion();
            onSuccessfullyLoaded(question);
        }
        catch (Exception ex)
        {
            if (onError != null)
            {
                onError(ex);    
            }
        }
    }

    public void GetNextQuestion(Action<ISimpleQuestion> onSuccessfullyLoaded, Action<Exception> onError = null)
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

    public void GetRandomQuestion(Action<ISimpleQuestion> onSuccessfullyLoaded, Action<Exception> onError = null)
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

