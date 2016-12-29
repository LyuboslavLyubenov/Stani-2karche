﻿using System;

namespace Assets.Scripts.Interfaces
{
    using EventArgs;

    public interface IGameDataIterator
    {
        event EventHandler<MarkEventArgs> OnMarkIncrease;

        event EventHandler OnLoaded;

        int RemainingQuestionsToNextMark
        {
            get;
        }

        int CurrentMark
        {
            get;
        }

        int SecondsForAnswerQuestion
        {
            get;    
        }

        string LevelCategory
        {
            get;
        }

        void GetCurrentQuestion(Action<ISimpleQuestion> onSuccessfullyLoaded, Action<Exception> onError = null);

        void GetNextQuestion(Action<ISimpleQuestion> onSuccessfullyLoaded, Action<Exception> onError = null);
    }

}