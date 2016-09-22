using System.Collections;
using System.Collections.Generic;
using System;

public class RemainingTimeToAnswerCommand : INetworkManagerCommand
{
    Action<RemainingTimeEventArgs> onReceivedRemainingTime;

    public RemainingTimeToAnswerCommand(Action<RemainingTimeEventArgs> onReceivedRemainingTime)
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
