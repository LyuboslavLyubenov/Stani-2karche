﻿namespace Assets.Tests.DummyObjects
{

    using System;

    using Assets.Scripts.EventArgs;
    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Interfaces.GameData;

    public class DummyGameDataIterator : IGameDataIterator
    {
        public event EventHandler<MarkEventArgs> OnMarkIncrease = delegate { };

        public event EventHandler OnLoaded = delegate { };

        public int RemainingQuestionsToNextMark
        {
            get; set;
        }

        public int CurrentMark
        {
            get; set;
        }

        public int SecondsForAnswerQuestion
        {
            get; set;
        }

        public string LevelCategory
        {
            get; set;
        }

        public bool Loaded
        {
            get; set;
        }

        public ISimpleQuestion CurrentQuestion
        {
            get; set;
        }

        public ISimpleQuestion NextQuestion
        {
            get; set;
        }

        public void GetCurrentQuestion(Action<ISimpleQuestion> onSuccessfullyLoaded, Action<Exception> onError = null)
        {
            onSuccessfullyLoaded(this.CurrentQuestion);
        }

        public void GetNextQuestion(Action<ISimpleQuestion> onSuccessfullyLoaded, Action<Exception> onError = null)
        {
            onSuccessfullyLoaded(this.NextQuestion);
        }

        public void IncreaseMark(int mark)
        {
            this.OnMarkIncrease(this, new MarkEventArgs(mark));
        }

        public void ExecuteOnLoaded()
        {
            this.OnLoaded(this, EventArgs.Empty);
        }
    }
}
