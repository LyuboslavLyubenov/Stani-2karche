using UnityEngine;
using System;

public class GameDataSender : MonoBehaviour
{
    public EventHandler<ServerSentQuestionEventArgs> OnSentQuestion = delegate
    {
    };
    
    public LocalGameData LocalGameData;
    public ServerNetworkManager NetworkManager;

    void Start()
    {
        LocalGameData.OnMarkIncrease += (sender, args) =>
        {
            var commandData = new NetworkCommandData("GameDataMark");
            commandData.AddOption("Mark", args.Mark.ToString());

            NetworkManager.SendAllClientsCommand(commandData);
        };

        var getCurrentQuestionCommand = new ReceivedGetCurrentQuestionCommand(LocalGameData, NetworkManager);
        var getRandomQuestionCommand = new ReceivedGetRandomQuestionCommand(LocalGameData, NetworkManager);
        var getNextQuestionCommand = new ReceivedGetNextQuestionCommand(LocalGameData, NetworkManager);
                
        getCurrentQuestionCommand.OnSentQuestion += (sender, args) => OnSentQuestionToClient(args);
        getRandomQuestionCommand.OnSentQuestion += (sender, args) => OnSentQuestionToClient(args);
        getNextQuestionCommand.OnSentQuestion += (sender, args) => OnSentQuestionToClient(args);

        NetworkManager.CommandsManager.AddCommand("GameDataGetQuestion", new GameDataGetQuestionRouterCommand(NetworkManager));
        NetworkManager.CommandsManager.AddCommand("GameDataGetCurrentQuestion", getCurrentQuestionCommand);
        NetworkManager.CommandsManager.AddCommand("GameDataGetRandomQuestion", getRandomQuestionCommand);
        NetworkManager.CommandsManager.AddCommand("GameDataGetNextQuestion", getNextQuestionCommand);
    }

    void OnSentQuestionToClient(ServerSentQuestionEventArgs args)
    {
        OnSentQuestion(this, args);
    }

}