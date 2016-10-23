using UnityEngine;
using System;
using System.Collections.Generic;

public class ReceivedAvailableCategoriesCommand : IOneTimeExecuteCommand
{
    Action<string[]> onGetAllCategories;

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

    public ReceivedAvailableCategoriesCommand(Action<string[]> onGetAllCategories)
    {
        if (onGetAllCategories == null)
        {
            throw new ArgumentNullException("onGetAllCategories");
        }
            
        this.onGetAllCategories = onGetAllCategories;
    }

    public void Execute(Dictionary<string, string> commandsOptionsValues)
    {
        var allCategories = commandsOptionsValues["AvailableCategories"].Split(',');
        onGetAllCategories(allCategories);
       
        if (OnFinishedExecution != null)
        {
            OnFinishedExecution(this, EventArgs.Empty);
        }

        FinishedExecution = true;
    }
}