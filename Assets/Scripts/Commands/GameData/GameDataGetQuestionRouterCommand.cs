namespace Assets.Scripts.Commands.GameData
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Assets.Scripts.Interfaces.Network.NetworkManager;

    using Network.NetworkManagers;

    using Interfaces;
    using Network;

    public class GameDataGetQuestionRouterCommand : INetworkManagerCommand
    {
        private IServerNetworkManager networkManager;

        public GameDataGetQuestionRouterCommand(IServerNetworkManager networkManager)
        {
            if (networkManager == null)
            {
                throw new ArgumentNullException("networkManager");
            }

            this.networkManager = networkManager;
        }

        public void Execute(Dictionary<string, string> commandsOptionsValues)
        {
            var requestType = (QuestionRequestType)Enum.Parse(typeof(QuestionRequestType), commandsOptionsValues["RequestType"]);

            switch (requestType)
            {
                case QuestionRequestType.Current:
                    var currentQuestionCommandData = new NetworkCommandData("GameDataGetCurrentQuestion");
                    this.MapOptionsToCommand(currentQuestionCommandData, commandsOptionsValues);
                    this.networkManager.CommandsManager.Execute(currentQuestionCommandData);
                    break;

                case QuestionRequestType.Next:
                    var nextQuestionCommandData = new NetworkCommandData("GameDataGetNextQuestion");
                    this.MapOptionsToCommand(nextQuestionCommandData, commandsOptionsValues);
                    this.networkManager.CommandsManager.Execute(nextQuestionCommandData);
                    break;

                default:
                    throw new NotImplementedException();
                    break;
            }
        }

        private void MapOptionsToCommand(NetworkCommandData commandData, Dictionary<string, string> options)
        {
            var optionsList = options.ToArray();

            for (int i = 0; i < optionsList.Length; i++)
            {
                var optionData = optionsList[i];
                commandData.AddOption(optionData.Key, optionData.Value);
            }
        }
    }
}