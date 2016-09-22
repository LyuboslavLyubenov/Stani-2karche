using System;
using System.Collections.Generic;

public class ReceivedNoMoreQuestionsCommand : INetworkManagerCommand
{
    Action onNoMoreQuestionsCommand;

    public ReceivedNoMoreQuestionsCommand(Action onNoMoreQuestionsCommand)
    {
        if (onNoMoreQuestionsCommand == null)
        {
            throw new ArgumentNullException("onNoMoreQuestionsCommand");
        }

        this.onNoMoreQuestionsCommand = onNoMoreQuestionsCommand;
    }

    public void Execute(Dictionary<string, string> commandsOptionsValues)
    {
        onNoMoreQuestionsCommand();
    }
}