using UnityEngine;
using System.IO;
using System.Linq;
using System;
using System.Collections.Generic;

public class ReceivedServerGetAllAvailableCategoriesCommand : INetworkManagerCommand
{
    readonly string[] RequiredFiles = new string[] { "3.xls", "4.xls", "5.xls", "6.xls", "Rating.csv" };

    ServerNetworkManager networkManager;

    public ReceivedServerGetAllAvailableCategoriesCommand(ServerNetworkManager networkManager)
    {
        if (networkManager == null)
        {
            throw new ArgumentNullException("networkManager");
        }
            
        this.networkManager = networkManager;
    }

    public void Execute(Dictionary<string, string> commandsOptionsValues)
    {
        var connectionId = int.Parse(commandsOptionsValues["ConnectionId"]);
        SendAvailableCategories(connectionId);
    }

    void SendAvailableCategories(int connectionId)
    {
        var localCategoriesReader = new LocalCategoriesReader();
        localCategoriesReader.GetAllCategories((categories) =>
            {
                var availableCategoriesCommand = new NetworkCommandData("AvailableCategories");
                availableCategoriesCommand.AddOption("AvailableCategories", string.Join(",", categories));

                networkManager.SendClientCommand(connectionId, availableCategoriesCommand);    
            });
    }
}