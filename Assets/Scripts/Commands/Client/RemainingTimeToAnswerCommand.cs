using System.Collections;
using System.Collections.Generic;
using System;

public class RemainingTimeToAnswerCommand : INetworkManagerCommand
{
    Action<RemainingTimeEventArgs> onReceivedRemainingTime;

    GameState currentGameState;

    public RemainingTimeToAnswerCommand(Action<RemainingTimeEventArgs> onReceivedRemainingTime, GameState currentGameState)
    { 
        if (onReceivedRemainingTime == null)
        {
            throw new ArgumentNullException("onReceivedRemainingTime");
        }

        this.onReceivedRemainingTime = onReceivedRemainingTime;
        this.currentGameState = currentGameState;
    }

    public void Execute(Dictionary<string, string> commandsOptionsValues)
    {
        if (currentGameState == GameState.Idle)
        {
            throw new Exception("Cant execute this command while in this gameState");
        }

        var secondsRemaining = int.Parse(commandsOptionsValues["SecondsRemaining"]);
        var remainingTimeEventArgs = new RemainingTimeEventArgs(secondsRemaining);

        onReceivedRemainingTime(remainingTimeEventArgs);
    }
}
