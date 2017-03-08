namespace Assets.Scripts.Commands.Client
{
    using System;
    using System.Collections.Generic;

    using Assets.Scripts.Interfaces.Network.NetworkManager;

    using Interfaces;

    using EventArgs = System.EventArgs;

    public class AvailableCategoriesCommand : IOneTimeExecuteCommand
    {
        private Action<string[]> onGetAllCategories;

        public EventHandler OnFinishedExecution
        {
            get;
            set;
        }

        public bool FinishedExecution
        {
            get;
            private set;
        }

        public AvailableCategoriesCommand(Action<string[]> onGetAllCategories)
        {
            if (onGetAllCategories == null)
            {
                throw new ArgumentNullException("onGetAllCategories");
            }
            
            this.onGetAllCategories = onGetAllCategories;
        }

        public void Execute(Dictionary<string, string> commandsOptionsValues)
        {
            var allCategories = commandsOptionsValues["AvailableCategories"].Split(',');
            this.onGetAllCategories(allCategories);
       
            if (this.OnFinishedExecution != null)
            {
                this.OnFinishedExecution(this, EventArgs.Empty);
            }

            this.FinishedExecution = true;
        }
    }
}