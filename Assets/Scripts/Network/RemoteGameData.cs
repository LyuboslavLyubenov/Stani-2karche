using System.Collections.Generic;
using System;
using UnityEngine;

public class RemoteGameData : IGameData
{
    public EventHandler OnLoadedGameData = delegate
    {
    };

    public bool Loaded
    {
        get
        {
            return loaded; 
        }
    }

    public int RemainingQuestionsToNextMark
    {
        get;
        private set;
    }

    public int CurrentMark
    {
        get;
        private set;
    }

    public int SecondsForAnswerQuestion
    {
        get;
        private set;
    }

    public string LevelCategory
    {
        get;
        private set;
    }

    ClientNetworkManager networkManager;

    bool loaded = false;

    Stack<PendingQuestionRequestData> currentQuestionRequests = new Stack<PendingQuestionRequestData>();
    Stack<PendingQuestionRequestData> nextQuestionRequests = new Stack<PendingQuestionRequestData>();
    Stack<PendingQuestionRequestData> randomQuestionRequests = new Stack<PendingQuestionRequestData>();

    ISimpleQuestion currentQuestionCache = null;

    public RemoteGameData(ClientNetworkManager networkManager)
    {
        this.networkManager = networkManager;
        InitializeCommands();
    }

    void InitializeCommands()
    {
        networkManager.CommandsManager.AddCommand("LoadedGameData", new LoadedGameDataCommand(_OnLoadedGameData));
        networkManager.CommandsManager.AddCommand("GameDataQuestion", new ReceivedQuestionCommand(OnReceivedQuestion));
        networkManager.CommandsManager.AddCommand("GameDataMark", new ReceivedMarkCommand(OnReceivedMark));
        networkManager.CommandsManager.AddCommand("GameDataNoMoreQuestions", new ReceivedNoMoreQuestionsCommand(OnNoMoreQuestions));
    }

    void _OnLoadedGameData(string levelCategory)
    {
        this.LevelCategory = levelCategory;
        OnLoadedGameData(this, EventArgs.Empty);
    }

    void OnNoMoreQuestions()
    {
        for (int i = 0; i < nextQuestionRequests.Count; i++)
        {
            var questionRequest = nextQuestionRequests.Pop();
            questionRequest.OnException(new NoMoreQuestionsException());
        }
    }

    void LoadDataFromServer()
    {
        GetCurrentQuestion((question) =>
            {
                currentQuestionCache = question;
            }, 
            Debug.LogException);
    }

    void OnReceivedMark(int mark)
    {
        CurrentMark = mark;

        if (OnMarkIncrease != null)
        {
            OnMarkIncrease(this, new MarkEventArgs(mark));    
        }
    }

    void OnReceivedQuestion(QuestionRequestType requestType, ISimpleQuestion question, int remainingQuestionsToNextMark, int secondsForAnswerQuestion)
    {
        PendingQuestionRequestData questionRequest = null;

        switch (requestType)
        {
            case QuestionRequestType.Current:
                questionRequest = currentQuestionRequests.PopOrDefault();
                break;
            case QuestionRequestType.Next:
                questionRequest = nextQuestionRequests.PopOrDefault();
                break;
            case QuestionRequestType.Random:
                questionRequest = randomQuestionRequests.PopOrDefault();
                break;
        }

        if (questionRequest == null)
        {
            Debug.LogWarning("Received question from server but cant find request source.");
            return;
        }
          
        if (requestType != QuestionRequestType.Random)
        {
            currentQuestionCache = question;
        }

        RemainingQuestionsToNextMark = remainingQuestionsToNextMark;
        SecondsForAnswerQuestion = secondsForAnswerQuestion;

        questionRequest.OnLoaded(question);
    }

    void SendGetQuestionRequest(QuestionRequestType requestType)
    {
        var commandData = new NetworkCommandData("GameDataGetQuestion");
        var requestTypeStr = Enum.GetName(typeof(QuestionRequestType), requestType);
        commandData.AddOption("RequestType", requestTypeStr);
        networkManager.SendServerCommand(commandData);
    }

    public void GetCurrentQuestion(Action<ISimpleQuestion> onSuccessfullyLoaded, Action<Exception> onError = null)
    {
        if (currentQuestionCache != null)
        {
            onSuccessfullyLoaded(currentQuestionCache);
            return;
        }

        try
        {
            SendGetQuestionRequest(QuestionRequestType.Current);

            var requestData = new PendingQuestionRequestData((question) => onSuccessfullyLoaded(question), (error) => onError(error));
            currentQuestionRequests.Push(requestData);
        }
        catch (Exception ex)
        {
            if (onError != null)
            {
                onError(ex);
            }
            else
            {
                throw;
            }
        }
    }

    public void GetNextQuestion(Action<ISimpleQuestion> onSuccessfullyLoaded, Action<Exception> onError = null)
    {
        try
        {
            SendGetQuestionRequest(QuestionRequestType.Next);

            var requestData = new PendingQuestionRequestData((question) => onSuccessfullyLoaded(question), (error) => onError(error));
            nextQuestionRequests.Push(requestData);
        }
        catch (Exception ex)
        {
            if (onError != null)
            {
                onError(ex);
            }
            else
            {
                throw;
            }
        }
    }

    public void GetRandomQuestion(Action<ISimpleQuestion> onSuccessfullyLoaded, Action<Exception> onError = null)
    {
        try
        {
            SendGetQuestionRequest(QuestionRequestType.Random);

            var requestData = new PendingQuestionRequestData((question) => onSuccessfullyLoaded(question), (error) => onError(error));
            randomQuestionRequests.Push(requestData);
        }
        catch (Exception ex)
        {
            if (onError != null)
            {
                onError(ex);
            }
            else
            {
                throw;
            }
        }
    }
}

public enum QuestionRequestType
{
    Current,
    Next,
    Random
}