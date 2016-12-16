using UnityEngine;
using System;
using System.Collections;
using UnityEngine.SceneManagement;

//Mediator
using System.Collections.Generic;

public class DummyCommandReceivedDataEventArgs : EventArgs
{
    public Dictionary<string, string> CommandsOptionsValues
    {
        get;
        private set;
    }

    public DummyCommandReceivedDataEventArgs(Dictionary<string, string> commandsOptionsValues)
    {
        if (commandsOptionsValues == null)
        {
            throw new ArgumentNullException("commandsOptionsValues");
        }
            
        this.CommandsOptionsValues = commandsOptionsValues;
    }
}