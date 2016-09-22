using System.Collections;
using System.Collections.Generic;
using System;

public class ReceivedRemainingTimeToAnswerCommand : INetworkManagerCommand
{
    Action<RemainingTimeEventArgs> onReceivedRemainingTime;

    public ReceivedRemainingTimeToAnswerCommand(Action<RemainingTimeEventArgs> onReceivedRemainingTime)
    { 
        if (onReceivedRemainingTime == null)
        {
            throw new ArgumentNullException("onReceivedRemainingTime");
        }

        this.onReceivedRemainingTime = onReceivedRemainingTime;
    }

    public void Execute(Dictionary<string, string> commandsOptionsValues)
    {
        var secondsRemaining = int.Parse(commandsOptionsValues["TimeInSeconds"]);
        var remainingTimeEventArgs = new RemainingTimeEventArgs(secondsRemaining);

        onReceivedRemainingTime(remainingTimeEventArgs);
    }
}
