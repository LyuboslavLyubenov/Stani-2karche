namespace Assets.Scripts.Commands.Client
{
    using System;
    using System.Collections.Generic;

    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Interfaces.Network.NetworkManager;

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
