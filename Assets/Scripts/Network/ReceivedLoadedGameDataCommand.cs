using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;

public class ReceivedLoadedGameDataCommand : INetworkManagerCommand
{
    public delegate void OnLoadedGameData(string levelCategory);

    OnLoadedGameData onLoadedGameData;

    public ReceivedLoadedGameDataCommand(OnLoadedGameData onLoadedGameData)
    {
        if (onLoadedGameData == null)
        {
            throw new ArgumentNullException("onLoadedGameData");
        }
            
        this.onLoadedGameData = onLoadedGameData;
    }

    public void Execute(Dictionary<string, string> commandsOptionsValues)
    {
        var category = commandsOptionsValues["LevelCategory"];
        onLoadedGameData(category);
    }
    
}