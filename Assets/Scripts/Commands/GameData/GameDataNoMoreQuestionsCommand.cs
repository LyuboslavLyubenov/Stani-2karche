using System;
using System.Collections.Generic;

public class GameDataNoMoreQuestionsCommand : INetworkManagerCommand
{
    Action onNoMoreQuestionsCommand;

    public GameDataNoMoreQuestionsCommand(Action onNoMoreQuestionsCommand)
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