using System;

public interface IGameData
{
    EventHandler<MarkEventArgs> OnMarkIncrease
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

    void GetCurrentQuestion(Action<ISimpleQuestion> onSuccessfullyLoaded, Action<Exception> onError = null);

    void GetNextQuestion(Action<ISimpleQuestion> onSuccessfullyLoaded, Action<Exception> onError = null);

    void GetRandomQuestion(Action<ISimpleQuestion> onSuccessfullyLoaded, Action<Exception> onError = null);
}
