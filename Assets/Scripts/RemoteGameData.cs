﻿using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;

public class RemoteGameData : MonoBehaviour, IGameData
{
    public EventHandler<MarkEventArgs> OnMarkIncrease
    {
        get;
        set;
    }

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

    public ClientNetworkManager NetworkManager;

    bool loaded = false;

    Stack<PendingQuestionRequestData> currentQuestionRequests = new Stack<PendingQuestionRequestData>();
    Stack<PendingQuestionRequestData> nextQuestionRequests = new Stack<PendingQuestionRequestData>();
    Stack<PendingQuestionRequestData> randomQuestionRequests = new Stack<PendingQuestionRequestData>();

    Question currentQuestionCache = null;

    void Start()
    {
        InitializeCommands();
        LoadDataFromServer();
    }

    void InitializeCommands()
    {
        NetworkManager.CommandsManager.AddCommand("GameDataQuestion", new ReceivedQuestionCommand(OnReceivedQuestion));
        NetworkManager.CommandsManager.AddCommand("GameDataMark", new ReceivedMarkCommand(OnReceivedMark));
    }

    void InititalizeGameDataCallback(Question firstQuestion, int questionsRemainingToNextMark)
    {
        currentQuestionCache = firstQuestion;
        RemainingQuestionsToNextMark = questionsRemainingToNextMark;
        loaded = true;
    }

    void LoadDataFromServer()
    {
        GetCurrentQuestion((question) =>
            {
                currentQuestionCache = question;
            }, (exception) =>
            {
                DebugUtils.LogException(exception);
                //TODO:
            });
    }

    void OnReceivedMark(int mark)
    {
        if (OnMarkIncrease != null)
        {
            OnMarkIncrease(this, new MarkEventArgs(mark));    
        }
    }

    void OnReceivedQuestion(QuestionRequestType requestType, Question question, int remainingQuestionsToNextMark)
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

        if (question == null)
        {
            var exception = new NoMoreQuestionsException("Cant load question");
            questionRequest.OnException(exception);
            return;
        }

        if (requestType != QuestionRequestType.Random)
        {
            currentQuestionCache = question;
        }

        RemainingQuestionsToNextMark = remainingQuestionsToNextMark;

        questionRequest.OnLoaded(question);
    }

    public void GetCurrentQuestion(Action<Question> onSuccessfullyLoaded, Action<Exception> onError = null)
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

    public void GetNextQuestion(Action<Question> onSuccessfullyLoaded, Action<Exception> onError = null)
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

    public void GetRandomQuestion(Action<Question> onSuccessfullyLoaded, Action<Exception> onError = null)
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

    void SendGetQuestionRequest(QuestionRequestType requestType)
    {
        var commandData = new NetworkCommandData("GameDataGetQuestion");
        var requestTypeStr = Enum.GetName(typeof(QuestionRequestType), requestType);
        commandData.AddOption("RequestType", requestTypeStr);
        NetworkManager.SendServerCommand(commandData);
    }
}

public enum QuestionRequestType
{
    Current,
    Next,
    Random
}
