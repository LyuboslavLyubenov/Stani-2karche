using System.Collections.Generic;
using System;

public class LoadedGameDataCommand : INetworkManagerCommand
{
    public delegate void OnLoadedGameData(string levelCategory);

    OnLoadedGameData onLoadedGameData;

    public LoadedGameDataCommand(OnLoadedGameData onLoadedGameData)
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