namespace Assets.Scripts.Interfaces.GameData
{

    using System;

    using Assets.Scripts.EventArgs;

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

        bool Loaded
        {
            get;
        }

        void GetCurrentQuestion(Action<ISimpleQuestion> onSuccessfullyLoaded, Action<Exception> onError = null);

        void GetNextQuestion(Action<ISimpleQuestion> onSuccessfullyLoaded, Action<Exception> onError = null);
    }

}
