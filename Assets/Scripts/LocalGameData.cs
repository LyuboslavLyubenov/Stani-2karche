using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using CSharpJExcel.Jxl;

using UnityEngine;

namespace Assets.Scripts
{

    using Assets.CielaSpike.Thread_Ninja;
    using Assets.Scripts.EventArgs;
    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Localization;
    using Assets.Scripts.Statistics;
    using Assets.Scripts.Utils;

    public class LocalGameData : MonoBehaviour, IGameData
    {

        public const string LevelPath = "LevelData\\теми\\";
        const int DefaultSecondsForAnswerQuestion = 60;
        const int DefaultQuestionToTakePerMark = int.MaxValue;
        public const int MarkMin = 3;
        public const int MarkMax = 6;

        public static readonly JExcelCellPosition SettingsStartPosition = new JExcelCellPosition(0, 5);

        readonly JExcelCellPosition QuestionsToTakePosition = new JExcelCellPosition(1, 1);
        readonly JExcelCellPosition SecondsForAnswerQuestionPosition = new JExcelCellPosition(3, 1);

        /// <summary>
        /// If true questions for given marks are aways with randomized order
        /// </summary>
        public bool ShuffleQuestions = true;
        /// <summary>
        /// If true answers for every questions will be in random arrangement
        /// </summary>
        public bool ShuffleAnswers = true;

        public string LevelCategory
        {
            get;
            set;
        }

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
                if (!this.Loaded)
                {
                    throw new Exception("Not loaded");
                }

                return this.questionsToTakePerMark[this.currentMarkIndex] - this.currentQuestionIndex - 1;    
            }
        }

        public bool IsLastQuestion
        {
            get
            {
                if (!this.Loaded)
                {
                    throw new Exception("Not loaded");
                }

                return (this.currentMarkIndex < this.marksQuestions.Count) && ((this.currentQuestionIndex + 1) >= this.marksQuestions[this.currentMarkIndex].Length);
            }
        }

        public int CurrentMark
        {
            get
            {
                if (!this.Loaded)
                {
                    throw new Exception("Not loaded");
                }
            
                return this.currentMarkIndex + (MarkMin - 1);
            }
        }

        public int SecondsForAnswerQuestion
        {
            get
            {
                if (!this.Loaded)
                {
                    throw new Exception("Not loaded");
                }

                return this.secondsForAnswerQuestionPerMark[this.currentMarkIndex];
            }
        }

        int currentQuestionIndex = 0;
        int currentMarkIndex = 0;

        List<ISimpleQuestion[]> marksQuestions = new List<ISimpleQuestion[]>();
        List<int> questionsToTakePerMark = new List<int>();
        List<int> secondsForAnswerQuestionPerMark = new List<int>();

        void Awake()
        {
            this.OnLoaded = delegate
                {
                };

            this.OnMarkIncrease = delegate
                {
                };
        }

        IEnumerator ExtractLevelDataAsync(Action<Exception> onError)
        {
            Exception exception = null;

            try
            {
                this.Loaded = false;
                this.Loading = true;
                this.ExtractLevelData();
                this.Loaded = true;
                this.Loading = false;
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            yield return Ninja.JumpToUnity;

            if (exception == null)
            {
                this.OnLoaded(this, System.EventArgs.Empty);    
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
            var execPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\..\\..\\";
            var levelPath = execPath + LevelPath + this.LevelCategory;
            var questionFilesPath = Directory.GetFiles(levelPath).Where(p => p.EndsWith(".xls")).ToArray();

            for (int i = 0; i < questionFilesPath.Length; i++)
            {
                var markQuestionsDataPath = questionFilesPath[i];
                var workbook = Workbook.getWorkbook(new FileInfo(markQuestionsDataPath));
                var sheet = workbook.getSheet(0);
                int questionsToTake = DefaultQuestionToTakePerMark;
                int secondsForAnswerQuestion = DefaultSecondsForAnswerQuestion;
                ISimpleQuestion[] allQuestionsForMark;

                allQuestionsForMark = this.ExtractQuestionsFromWorksheet(sheet, i);    

                try
                {
                    questionsToTake = int.Parse(sheet.getCell(this.QuestionsToTakePosition.Column, this.QuestionsToTakePosition.Row).getContents());
                }
                catch (Exception ex)
                {
                    var fileName = markQuestionsDataPath.Split('/').Last();
                    var errorMsg = LanguagesManager.Instance.GetValue("Errors/CantExtractQuestionsToTakeCount");

                    UnityEngine.Debug.LogFormat(errorMsg, fileName);
                }

                try
                {
                    secondsForAnswerQuestion = int.Parse(sheet.getCell(this.SecondsForAnswerQuestionPosition.Column, this.SecondsForAnswerQuestionPosition.Row).getContents());    
                }
                catch (Exception ex)
                {
                    var fileName = markQuestionsDataPath.Split('/').Last();
                    var cantExtractSecondsForAnswerMsg = LanguagesManager.Instance.GetValue("Errors/CantExtractSecondsForAnswer");
                    UnityEngine.Debug.LogFormat(cantExtractSecondsForAnswerMsg, fileName, DefaultSecondsForAnswerQuestion);
                }

                this.secondsForAnswerQuestionPerMark.Add(secondsForAnswerQuestion);
                this.questionsToTakePerMark.Add(questionsToTake);

                if (this.ShuffleQuestions)
                {
                    allQuestionsForMark.Shuffle();
                }

                this.marksQuestions.Add(allQuestionsForMark);
            }
        }

        ISimpleQuestion[] ExtractQuestionsFromWorksheet(Sheet sheet, int workbookMarkIndex)
        {
            var questions = new List<ISimpleQuestion>();

            for (int rowi = LocalGameData.SettingsStartPosition.Row; rowi < sheet.getRows();)
            {
                var questionText = sheet.GetCellOrDefault(0, rowi).getContents().Trim();

                if (string.IsNullOrEmpty(questionText))
                {
                    break;
                }

                var answers = new List<string>();
                var correctAnswer = "";

                for (int answersRowI = rowi + 1;; answersRowI++)
                {   
                    var answerText = sheet.GetCellOrDefault(0, answersRowI).getContents().Trim();
                    var isCorrect = sheet.GetCellOrDefault(1, answersRowI).getContents().ToUpperInvariant() == ("верен").ToUpperInvariant();

                    if (string.IsNullOrEmpty(answerText))
                    {                
                        rowi = answersRowI + 1;
                        break;
                    }

                    answers.Add(answerText);

                    if (isCorrect)
                    {
                        if (!string.IsNullOrEmpty(correctAnswer))
                        {
                            var errorMsg = LanguagesManager.Instance.GetValue("Errors/MultipleCorrectAnswers");
                            this.ExtractingQuestionException(workbookMarkIndex, questions.Count + 1, errorMsg);   
                        }

                        correctAnswer = answerText;
                    }
                }

                if (string.IsNullOrEmpty(correctAnswer))
                {
                    var errorMsg = LanguagesManager.Instance.GetValue("Errors/NoCorrectAnswer");
                    this.ExtractingQuestionException(workbookMarkIndex, questions.Count + 1, errorMsg);
                }

                if (answers.Count <= 0)
                {
                    var errorMsg = LanguagesManager.Instance.GetValue("Errors/QuestionWithoutAnswers");
                    this.ExtractingQuestionException(workbookMarkIndex, questions.Count + 1, errorMsg);
                }

                if (this.ShuffleAnswers)
                {
                    answers.Shuffle();
                }

                var correctAnswerIndex = answers.IndexOf(correctAnswer);
                var question = new SimpleQuestion(questionText, answers.ToArray(), correctAnswerIndex);

                questions.Add(question);
            }

            return questions.ToArray();
        }

        void ExtractingQuestionException(int workbookMarkIndex, int questionNumber, string exceptionMsg)
        {
            var fileName = (MarkMin + workbookMarkIndex) + ".xls";
            var errorFormatedMsg = string.Format(exceptionMsg, fileName, questionNumber);
            throw new Exception(errorFormatedMsg); 
        }

        ISimpleQuestion _GetCurrentQuestion()
        {
            if (!this.Loaded)
            {
                var errorMsg = LanguagesManager.Instance.GetValue("Errors/NotLoadedQuestions");
                throw new Exception(errorMsg);
            }

            if (this.currentMarkIndex >= this.marksQuestions.Count)
            {
                return null;
            }

            var questions = this.marksQuestions[this.currentMarkIndex];
            var index = Mathf.Min(questions.Length - 1, this.currentQuestionIndex);
            return questions[index];
        }

        ISimpleQuestion _GetNextQuestion()
        {
            if (!this.Loaded)
            {
                var errorMsg = LanguagesManager.Instance.GetValue("Errors/NotLoadedQuestions");
                throw new Exception(errorMsg);
            }

            if (this.currentMarkIndex >= this.marksQuestions.Count)
            {
                return null;
            }

            var nextQuestionIndex = this.currentQuestionIndex + 1;
            var questions = this.marksQuestions[this.currentMarkIndex];
            var questionsToTake = this.questionsToTakePerMark[this.currentMarkIndex];

            if (questionsToTake > questions.Length)
            {
                questionsToTake = questions.Length;
            }

            if (nextQuestionIndex >= questionsToTake)
            {
                this.currentMarkIndex++;
                this.currentQuestionIndex = 0;

                if (this.OnMarkIncrease != null)
                {
                    this.OnMarkIncrease(this, new MarkEventArgs(this.currentMarkIndex + 2));
                }

                return this._GetCurrentQuestion();
            }
            else
            {
                this.currentQuestionIndex = nextQuestionIndex;

                var question = questions[this.currentQuestionIndex];
                return question;
            }
        }

        ISimpleQuestion _GetRandomQuestion()
        {
            if (!this.Loaded)
            {
                var errorMsg = LanguagesManager.Instance.GetValue("Errors/NotLoadedQuestions");
                throw new Exception(errorMsg);
            }

            var questionIndex = UnityEngine.Random.Range(0, this.marksQuestions[this.currentMarkIndex].Length);
            return this.marksQuestions[this.currentMarkIndex][questionIndex];
        }

        public void LoadDataAsync(Action<Exception> onErrorLoading)
        {
            if (this.Loading)
            {
                var errorMsg = LanguagesManager.Instance.GetValue("Errors/CurrentlyLoadingQuestions");
                onErrorLoading(new InvalidOperationException(errorMsg));
                return;
            }

            this.StartCoroutineAsync(this.ExtractLevelDataAsync(onErrorLoading));
        }

        public ISimpleQuestion GetQuestion(int markIndex, int questionIndex)
        {
            if (!this.Loaded)
            {
                throw new Exception("Not loaded");
            }

            if (markIndex < 0 || markIndex >= this.marksQuestions.Count)
            {
                throw new ArgumentOutOfRangeException("markIndex");
            }

            if (questionIndex < 0 || questionIndex >= this.marksQuestions[markIndex].Length)
            {
                throw new ArgumentOutOfRangeException("questionIndex");
            }

            var question = this.marksQuestions[markIndex][questionIndex];
            return question;
        }

        //TODO: Refactor. (something like) Iterator pattern. This class should contain only data extraction logic.     

        public void GetCurrentQuestion(Action<ISimpleQuestion> onSuccessfullyLoaded, Action<Exception> onError = null)
        {
            try
            {
                var question = this._GetCurrentQuestion();
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
                var question = this._GetNextQuestion();
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
                var question = this._GetRandomQuestion();
                onSuccessfullyLoaded(question);
            }
            catch (Exception ex)
            {
                onError(ex);
            }
        }
    }

}