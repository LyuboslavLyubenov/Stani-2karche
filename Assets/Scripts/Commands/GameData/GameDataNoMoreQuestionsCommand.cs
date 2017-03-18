namespace Commands.GameData
{

    using System;
    using System.Collections.Generic;

    using Interfaces.Network.NetworkManager;

    public class GameDataNoMoreQuestionsCommand : INetworkManagerCommand
    {
        private Action onNoMoreQuestionsCommand;

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
            this.onNoMoreQuestionsCommand();
        }
    }
}