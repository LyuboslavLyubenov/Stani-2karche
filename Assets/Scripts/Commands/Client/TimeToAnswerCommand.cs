namespace Commands.Client
{

    using System;
    using System.Collections.Generic;

    using Interfaces.Network.NetworkManager;

    public class TimeToAnswerCommand : INetworkManagerCommand
    {
        private Action<int> onReceivedRemainingTime;

        public TimeToAnswerCommand(Action<int> onReceivedRemainingTime)
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
