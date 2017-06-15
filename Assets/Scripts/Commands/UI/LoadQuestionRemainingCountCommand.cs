using INetworkManagerCommand = Interfaces.Network.NetworkManager.INetworkManagerCommand;

namespace Assets.Scripts.Commands.UI
{

    using System;
    using System.Collections.Generic;

    using Assets.Scripts.Extensions;
    using Assets.Scripts.Interfaces.Controllers;

    public class LoadQuestionRemainingCountCommand : INetworkManagerCommand
    {
        private readonly IQuestionsRemainingUIController questionsRemainingUIController;

        public LoadQuestionRemainingCountCommand(IQuestionsRemainingUIController questionsRemainingUIController)
        {
            if (questionsRemainingUIController == null)
            {
                throw new ArgumentNullException("questionsRemainingUIController");
            }

            this.questionsRemainingUIController = questionsRemainingUIController;
        }

        public void Execute(Dictionary<string, string> commandsOptionsValues)
        {
            var count = commandsOptionsValues["Count"]
                .ConvertTo<int>();
            this.questionsRemainingUIController.SetRemainingQuestions(count);
        }
    }
}
