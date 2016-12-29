﻿namespace Assets.Scripts.IO
{
    using System;

    using DTOs;

    using EventArgs;

    using Interfaces;
    using Localization;

    public class GameDataIterator : IGameDataIterator
    {
        public const int MarkMin = 2;

        public event EventHandler OnLoaded;

        public event EventHandler<MarkEventArgs> OnMarkIncrease;

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
            get
            {
                return this.extractor.LevelCategory;
            }
        }

        public bool Loaded
        {
            get
            {
                return this.extractor.Loaded;
            }
        }

        public bool Loading
        {
            get
            {
                return this.extractor.Loading;
            }
        }


        public int RemainingQuestionsToNextMark
        {
            get
            {
                if (!this.Loaded)
                {
                    throw new Exception("Not loaded");
                }

                var markQuestionsCount = this.extractor.GetQuestionsCountForMark(this.currentMarkIndex);
                return markQuestionsCount - (this.currentQuestionIndex + 1);
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

                if (this.currentMarkIndex >= this.extractor.MaxMarkIndex)
                {
                    return true;
                }
                
                return this.RemainingQuestionsToNextMark <= 0;
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

                return this.currentMarkIndex + MarkMin;
            }
            private set
            {
                this.currentMarkIndex = value;

                if (this.OnMarkIncrease != null)
                {
                    this.OnMarkIncrease(this, new MarkEventArgs(value));
                }
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

                return this.currentQuestion.SecondsForAnswerQuestion;
            }
        }

        private int currentQuestionIndex = 0;
        private int currentMarkIndex = 0;

        private ExtractedQuestion currentQuestion = null;
        private readonly GameDataExtractor extractor;

        public GameDataIterator()
        {
            this.extractor = new GameDataExtractor();
        }

        private ISimpleQuestion _GetCurrentQuestion()
        {
            if (!this.Loaded)
            {
                var errorMsg = LanguagesManager.Instance.GetValue("Errors/NotLoadedQuestions");
                throw new Exception(errorMsg);
            }

            if (this.currentMarkIndex > this.extractor.MaxMarkIndex)
            {
                return null;
            }
            
            var extractedQuestion = this.extractor.GetQuestion(this.currentMarkIndex, this.currentQuestionIndex);

            this.currentQuestion = extractedQuestion;

            return this.currentQuestion.Question;
        }

        private ISimpleQuestion _GetNextQuestion()
        {
            if (!this.Loaded)
            {
                var errorMsg = LanguagesManager.Instance.GetValue("Errors/NotLoadedQuestions");
                throw new Exception(errorMsg);
            }

            if (this.currentMarkIndex > this.extractor.MaxMarkIndex)
            {
                return null;
            }

            if (this.IsLastQuestion)
            {
                this.currentMarkIndex++;
                this.currentQuestionIndex = 0;
            }

            return this._GetCurrentQuestion();
        }
        
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
                if (onError != null)
                {
                    onError(ex);
                }
            }
        }
    }

}