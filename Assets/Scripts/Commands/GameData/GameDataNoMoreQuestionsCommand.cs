using System;
using System.Collections.Generic;

namespace Assets.Scripts.Commands.GameData
{

    using Assets.Scripts.Interfaces;

    public class GameDataNoMoreQuestionsCommand : INetworkManagerCommand
    {
        Action onNoMoreQuestionsCommand;

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