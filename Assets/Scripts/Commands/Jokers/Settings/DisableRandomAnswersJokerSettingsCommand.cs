using System.Linq;
using Interfaces.Controllers;

namespace Commands.Jokers
{
    using System;
    using System.Collections.Generic;

    using Interfaces.Network.NetworkManager;

    using EventArgs = System.EventArgs;

    public class DisableRandomAnswersJokerSettingsCommand : IOneTimeExecuteCommand
    {
        private IQuestionUIController questionUIController;

        public bool FinishedExecution
        {
            get;
            private set;
        }

        public EventHandler OnFinishedExecution
        {
            get;
            set;
        }

        public DisableRandomAnswersJokerSettingsCommand(IQuestionUIController questionUIController)
        {
            if (questionUIController == null)
            {
                throw new ArgumentNullException("questionUIController");
            }
                
            this.questionUIController = questionUIController;
        }

        public void Execute(Dictionary<string, string> commandsOptionsValues)
        {
            var answersToDisable = 
                commandsOptionsValues.Values.Select(ov => ov)
                    .ToArray();
            
            var currentQuestion = this.questionUIController.CurrentlyLoadedQuestion;
            var answersToHide = 
                currentQuestion.Answers.Where(answersToDisable.Contains)
                    .ToArray();

            for (int i = 0; i < answersToHide.Length; i++)
            {
                var answerToHide = answersToHide[i];
                this.questionUIController.HideAnswer(answerToHide);
            }

            this.FinishedExecution = true;

            if (this.OnFinishedExecution != null)
            {
                this.OnFinishedExecution(this, EventArgs.Empty);
            }
        }
    }
}