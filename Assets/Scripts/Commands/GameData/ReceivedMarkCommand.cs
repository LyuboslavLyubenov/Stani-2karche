using System;
using System.Collections.Generic;

public class ReceivedMarkCommand : INetworkManagerCommand
{
    Action<int> onReceivedMark;

    public ReceivedMarkCommand(Action<int> onReceivedMark)
    {
        if (onReceivedMark == null)
        {
            throw new ArgumentNullException("onReceivedMark");
        }

        this.onReceivedMark = onReceivedMark;
    }

    public void Execute(Dictionary<string, string> commandsOptionsValues)
    {
        var mark = int.Parse(commandsOptionsValues["Mark"]);
        onReceivedMark(mark);
    }
}