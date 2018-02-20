namespace IO
{

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using Assets.Scripts.Utils;

    using CielaSpike.Thread_Ninja;

    using CSharpJExcel.Jxl;

    using DTOs;

    using Exceptions;

    using Interfaces;
    using Interfaces.GameData;

    using Statistics;

    using Utils;

    public class GameDataExtractor : IGameDataExtractor
    {
        public const string LevelPath = "/LevelData/теми/";
        public const int QuestionsStartRow = 5;

        private const int DefaultSecondsForAnswerQuestion = 60;

        private const int DefaultQuestionToTakePerMark = int.MaxValue;

        private readonly JExcelCellPosition QuestionsToTakePosition = new JExcelCellPosition(1, 1);
        private readonly JExcelCellPosition SecondsForAnswerQuestionPosition = new JExcelCellPosition(3, 1);

        public event EventHandler OnLoaded = delegate { };

        /// <summary>
        /// If true questions for given marks are aways with random order
        /// </summary>
        public bool ShuffleQuestions
        {
            get; set;
        }
        /// <summary>
        /// If true answers for every questions will be in random arrangement
        /// </summary>
        public bool ShuffleAnswers
        {
            get; set;
        }

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

        public int MaxMarkIndex
        {
            get
            {
                return this.marksQuestions.Count - 1;
            }
        }

        private readonly List<ISimpleQuestion[]> marksQuestions = new List<ISimpleQuestion[]>();
        private readonly List<int> secondsForAnswerQuestionPerMark = new List<int>();

        private IEnumerator ExtractDataAsyncCoroutine(Action<Exception> onError)
        {
            Exception exception = null;

            try
            {
                this.Loaded = false;
                this.Loading = true;
                this.ExtractData();
                this.Loading = false;
                this.Loaded = true;
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            yield return Ninja.JumpToUnity;

            this.Loading = false;

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
        /// Excel files paths. (all excel files from LevelCategory path)
        /// </summary>
        private string[] GetQuestionsFilesPath()
        {
            var gameDirectoryPath = PathUtils.GetGameDirectoryPath();
            var levelPath = gameDirectoryPath + LevelPath + this.LevelCategory;
            var questionsFilesPath = Directory.GetFiles(levelPath).Where(p => p.EndsWith(".xls")).ToArray();
            return questionsFilesPath;
        }

        /// <summary>
        /// Load all questions and seperate them by categories
        /// </summary>
        private void ExtractData()
        {
            var questionsFilesPath = this.GetQuestionsFilesPath();

            for (int i = 0; i < questionsFilesPath.Length; i++)
            {
                var markQuestionsPath = questionsFilesPath[i];
                var workbook = Workbook.getWorkbook(new FileInfo(markQuestionsPath));
                var sheet = workbook.getSheet(0);
                int questionsToTake = DefaultQuestionToTakePerMark;
                int secondsForAnswerQuestion = DefaultSecondsForAnswerQuestion;

                try
                {
                    questionsToTake = int.Parse(sheet.getCell(this.QuestionsToTakePosition.Column, this.QuestionsToTakePosition.Row).getContents());
                }
                catch
                {
                    var fileName = markQuestionsPath.Split('/')
                        .Last();
                    UnityEngine.Debug.LogWarningFormat("Error while extracting questionsToTake from {0}. Using default value. (using all) ", fileName);
                }

                try
                {
                    secondsForAnswerQuestion = int.Parse(sheet.getCell(this.SecondsForAnswerQuestionPosition.Column, this.SecondsForAnswerQuestionPosition.Row).getContents());
                }
                catch
                {
                    var fileName = markQuestionsPath.Split('/').Last();
                    UnityEngine.Debug.LogErrorFormat("Error while extracting secondsForAnswerQuestion from {0}. Using default value ({1})", fileName, DefaultSecondsForAnswerQuestion);
                }

                this.secondsForAnswerQuestionPerMark.Add(secondsForAnswerQuestion);

                ISimpleQuestion[] allQuestionsForMark = this.ExtractQuestions(sheet, questionsToTake);

                this.marksQuestions.Add(allQuestionsForMark);
            }
        }

        private ISimpleQuestion[] ExtractQuestions(Sheet sheet, int questionsToTake)
        {
            var questions = new List<ISimpleQuestion>();

            for (int rowi = QuestionsStartRow; rowi < sheet.getRows();)
            {
                var questionText = sheet.GetCellOrDefault(0, rowi).getContents().Trim();

                if (string.IsNullOrEmpty(questionText))
                {
                    break;
                }

                var answersExtractionResult = this.ExtractAnswers(sheet, rowi);
                var question = new SimpleQuestion(questionText, answersExtractionResult.Answers, answersExtractionResult.CorrectAnswerIndex);

                questions.Add(question);

                rowi += question.Answers.Length + 2;
            }

            if (this.ShuffleQuestions)
            {
                return questions.GetRandomElements(questionsToTake).ToArray();                
            }
            else
            {
                return questions.ToArray();
            }
        }

        /// <summary>
        /// Extracts answers for question on rowi (row index). 
        /// Possible exceptions:
        /// * MutlipleCorrectAnswersException
        /// * NoCorrectAnswerException
        /// </summary>
        /// <param name="rowi">row index</param>
        /// <returns></returns>
        private AnswerExtractionResult ExtractAnswers(Sheet sheet, int rowi)
        {
            var answers = new List<string>();
            var correctAnswer = string.Empty;

            for (int answersRowI = rowi + 1; ; answersRowI++)
            {
                var answerText = sheet.GetCellOrDefault(0, answersRowI).getContents().Trim();

                if (string.IsNullOrEmpty(answerText))
                {
                    break;
                }

                var isCorrect = sheet.GetCellOrDefault(1, answersRowI)
                    .getContents()
                    .ToUpperInvariant() == ("верен").ToUpperInvariant();

                answers.Add(answerText);

                if (isCorrect)
                {
                    if (!string.IsNullOrEmpty(correctAnswer))
                    {
                        throw new MutlipleCorrectAnswersException();
                    }

                    correctAnswer = answerText;
                }
            }

            if (answers.Count <= 0)
            {
                throw new NoAnswersToExtractException();
            }

            if (string.IsNullOrEmpty(correctAnswer))
            {
                throw new NoCorrectAnswerException();
            }

            if (this.ShuffleAnswers)
            {
                answers.Shuffle();
            }

            var correctAnswerIndex = answers.IndexOf(correctAnswer);
            return new AnswerExtractionResult(answers.ToArray(), correctAnswerIndex);
        }

        public void ExtractDataAsync(Action<Exception> onError)
        {
            ThreadUtils.Instance.RunOnBackgroundThread(this.ExtractDataAsyncCoroutine(onError));
        }

        public void ExtractDataSync()
        {
            this.Loaded = false;
            this.Loading = true;
            this.ExtractData();
            this.Loaded = true;
            this.Loading = false;

            this.OnLoaded(this, System.EventArgs.Empty);
        }

        public ExtractedQuestion GetQuestion(int markIndex, int questionIndex)
        {
            if (markIndex < 0 || markIndex >= this.marksQuestions.Count)
            {
                throw new ArgumentOutOfRangeException("markIndex");
            }

            var markQuestions = this.marksQuestions[markIndex];

            if (questionIndex < 0 || questionIndex >= markQuestions.Length)
            {
                throw new ArgumentOutOfRangeException("questionIndex");
            }

            var question = markQuestions[questionIndex];
            var secondsForAnswerQuestion = this.secondsForAnswerQuestionPerMark[markIndex];

            return new ExtractedQuestion(question, secondsForAnswerQuestion);
        }

        public int GetQuestionsCountForMark(int markIndex)
        {
            if (markIndex < 0 || markIndex >= this.marksQuestions.Count)
            {
                throw new ArgumentOutOfRangeException("markIndex");
            }

            return this.marksQuestions[markIndex].Length;
        }
    }
}