﻿using System;

public class PendingQuestionRequestData
{
    public delegate void OnSuccessfullyLoadedDelegate(Question question);

    public delegate void OnExceptionDelegate(Exception exception);

    public OnSuccessfullyLoadedDelegate OnLoaded
    {
        get;
        private set;
    }

    public OnExceptionDelegate OnException
    {
        get;
        private set;
    }

    public PendingQuestionRequestData(OnSuccessfullyLoadedDelegate onSuccessfullyLoaded, OnExceptionDelegate onException)
    {
        if (onSuccessfullyLoaded == null)
        {
            throw new ArgumentNullException("onSuccessfullyLoaded");
        }

        this.OnLoaded = onSuccessfullyLoaded;
        this.OnException = onException;
    }
}