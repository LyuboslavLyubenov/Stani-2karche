﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class InitializeGameDataCallbackCommand : INetworkManagerCommand
{
    public delegate void InitializeCallbackDelegate(Question firstQuestion,int questionsRemainingToNextMark);

    InitializeCallbackDelegate initializaCallback;

    public InitializeGameDataCallbackCommand(InitializeCallbackDelegate initializeCallback)
    {
        if (initializeCallback == null)
        {
            throw new ArgumentNullException("initializeCallback");
        }

        this.initializaCallback = initializeCallback;
    }

    public void Execute(Dictionary<string, string> commandsOptionsValues)
    {
        var questionJSON = commandsOptionsValues["QuestionJSON"];
        var question = JsonUtility.FromJson<Question>(questionJSON);
        var questionsRemainingToNextMark = int.Parse(commandsOptionsValues["RemainingQuestionsToNextMark"]);

        initializaCallback(question, questionsRemainingToNextMark);
    }

}