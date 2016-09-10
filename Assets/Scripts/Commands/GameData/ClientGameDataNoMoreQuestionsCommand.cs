using System;
using System.Collections.Generic;

public class ClientGameDataNoMoreQuestionsCommand : INetworkManagerCommand
{
    Action onNoMoreQuestionsCommand;

    public ClientGameDataNoMoreQuestionsCommand(Action onNoMoreQuestionsCommand)
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