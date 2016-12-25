using System;
using System.Collections.Generic;

namespace Assets.Scripts.Commands.Client
{

    using Assets.Scripts.Interfaces;

    public class ReceivedTimeToAnswerCommand : INetworkManagerCommand
    {
        Action<int> onReceivedRemainingTime;

        public ReceivedTimeToAnswerCommand(Action<int> onReceivedRemainingTime)
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
            this.onReceivedRemainingTime(secondsRemaining);
        }
    }

}
