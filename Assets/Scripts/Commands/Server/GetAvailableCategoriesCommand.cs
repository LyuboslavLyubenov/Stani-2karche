namespace Commands.Server
{

    using System;
    using System.Collections.Generic;

    using Interfaces;
    using Interfaces.Network.NetworkManager;

    using IO;

    using EventArgs = System.EventArgs;

    public class GetAvailableCategoriesCommand : IOneTimeExecuteCommand
    {
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

        private readonly IServerNetworkManager networkManager;
        private readonly IAvailableCategoriesReader categoriesReader;

        public GetAvailableCategoriesCommand(IServerNetworkManager networkManager, IAvailableCategoriesReader categoriesReader)
        {
            if (networkManager == null)
            {
                throw new ArgumentNullException("networkManager");
            }
            
            this.networkManager = networkManager;
            this.categoriesReader = categoriesReader;
        }

        public void Execute(Dictionary<string, string> commandsOptionsValues)
        {
            var connectionId = int.Parse(commandsOptionsValues["ConnectionId"]);
            this.SendAvailableCategories(connectionId);

            this.FinishedExecution = true;

            if (this.OnFinishedExecution != null)
            {
                this.OnFinishedExecution(this, EventArgs.Empty);
            }
        }

        private void SendAvailableCategories(int connectionId)
        {
            this.categoriesReader.GetAllCategories((categories) =>
                {
                    var availableCategoriesCommand = new NetworkCommandData("AvailableCategories");
                    availableCategoriesCommand.AddOption("AvailableCategories", string.Join(",", categories));

                    this.networkManager.SendClientCommand(connectionId, availableCategoriesCommand);    
                });
        }
    }

}