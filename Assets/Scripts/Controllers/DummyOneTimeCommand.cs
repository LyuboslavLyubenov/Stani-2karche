using UnityEngine;
using System;
using System.Collections;
using UnityEngine.SceneManagement;

//Mediator

public class DummyOneTimeCommand : IOneTimeExecuteCommand
{
    public EventHandler OnFinishedExecution
    {
        get;
        set;
    }

    public bool FinishedExecution
    {
        get;
        private set;
    }

    public void Execute(System.Collections.Generic.Dictionary<string, string> commandsOptionsValues)
    {
        if (OnFinishedExecution != null)
        {
            OnFinishedExecution(this, EventArgs.Empty);
        }

        FinishedExecution = true;
    }
}