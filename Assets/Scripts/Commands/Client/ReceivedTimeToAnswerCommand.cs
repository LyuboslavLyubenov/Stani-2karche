namespace Assets.Scripts.Commands.Client
{
    using System;
    using System.Collections.Generic;

    using Assets.Scripts.Interfaces;

    public class ReceivedTimeToAnswerCommand : INetworkManagerCommand
    {
        private Action<int> onReceivedRemainingTime;

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
