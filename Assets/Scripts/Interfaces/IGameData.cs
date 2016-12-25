using System;

namespace Assets.Scripts.Interfaces
{

    using Assets.Scripts.EventArgs;

    public interface IGameData
    {
        EventHandler<MarkEventArgs> OnMarkIncrease
        {
            get;
            set;
        }

        EventHandler OnLoaded
        {
            get;
            set;
        }

        bool Loaded
        {
            get;
        }

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

        void GetRandomQuestion(Action<ISimpleQuestion> onSuccessfullyLoaded, Action<Exception> onError = null);
    }

}
