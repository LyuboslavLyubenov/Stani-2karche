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

    void GetCurrentQuestion(Action<Question> onSuccessfullyLoaded, Action<Exception> onError = null);

    void GetNextQuestion(Action<Question> onSuccessfullyLoaded, Action<Exception> onError = null);

    void GetRandomQuestion(Action<Question> onSuccessfullyLoaded, Action<Exception> onError = null);
}
