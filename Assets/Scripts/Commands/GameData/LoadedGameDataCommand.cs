namespace Assets.Scripts.Commands.GameData
{

    using System;
    using System.Collections.Generic;

    using Assets.Scripts.Interfaces;

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
            this.onLoadedGameData(category);
        }
    
    }

}