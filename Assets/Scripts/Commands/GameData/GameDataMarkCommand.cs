namespace Assets.Scripts.Commands.GameData
{
    using System;
    using System.Collections.Generic;

    using Interfaces;

    public class GameDataMarkCommand : INetworkManagerCommand
    {
        private Action<int> onReceivedMark;

        public GameDataMarkCommand(Action<int> onReceivedMark)
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
            this.onReceivedMark(mark);
        }
    }
}