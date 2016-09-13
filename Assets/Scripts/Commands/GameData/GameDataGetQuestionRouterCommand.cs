using System;
using System.Collections.Generic;
using System.Linq;

public class GameDataGetQuestionRouterCommand : INetworkManagerCommand
{
    ServerNetworkManager networkManager;

    public GameDataGetQuestionRouterCommand(ServerNetworkManager networkManager)
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
                MapOptionsToCommand(currentQuestionCommandData, commandsOptionsValues);
                networkManager.CommandsManager.Execute(currentQuestionCommandData);
                break;

            case QuestionRequestType.Next:
                var nextQuestionCommandData = new NetworkCommandData("GameDataGetNextQuestion");
                MapOptionsToCommand(nextQuestionCommandData, commandsOptionsValues);
                networkManager.CommandsManager.Execute(nextQuestionCommandData);
                break;

            case QuestionRequestType.Random:
                var randomQuestionCommandData = new NetworkCommandData("GameDataGetRandomQuestion");
                MapOptionsToCommand(randomQuestionCommandData, commandsOptionsValues);
                networkManager.CommandsManager.Execute(randomQuestionCommandData);
                break;

            default:
                throw new NotImplementedException();
                break;
        }
    }

    void MapOptionsToCommand(NetworkCommandData commandData, Dictionary<string, string> options)
    {
        var optionsList = options.ToList();

        foreach (var optionNameValue in optionsList)
        {
            commandData.AddOption(optionNameValue.Key, optionNameValue.Value);
        }
    }
}