using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public class GameDataSender : MonoBehaviour
{
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
                
        NetworkManager.CommandsManager.AddCommand("GameDataGetQuestion", new GameDataGetQuestionRouterCommand(NetworkManager));
        NetworkManager.CommandsManager.AddCommand("GameDataGetCurrentQuestion", new ReceivedGetCurrentQuestionCommand(LocalGameData, NetworkManager));
        NetworkManager.CommandsManager.AddCommand("GameDataGetRandomQuestion", new ReceivedGetRandomQuestionCommand(LocalGameData, NetworkManager));
        NetworkManager.CommandsManager.AddCommand("GameDataGetNextQuestion", new ReceivedGetNextQuestionCommand(LocalGameData, NetworkManager));
    }
}

