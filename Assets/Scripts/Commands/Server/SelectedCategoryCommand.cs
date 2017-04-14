namespace Commands.Server
{
    using System;

    using Interfaces;
    using Interfaces.GameData;
    using Interfaces.Network.NetworkManager;

    using EventArgs = System.EventArgs;

    public class SelectedCategoryCommand : IOneTimeExecuteCommand
    {
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

        private ILeaderboardDataManipulator leaderboard;

        private IGameDataExtractor gameData;

        public SelectedCategoryCommand(IGameDataExtractor gameData, ILeaderboardDataManipulator leaderboard)
        {
            if (gameData == null)
            {
                throw new ArgumentNullException("gameData");
            }
            
            if (leaderboard == null)
            {
                throw new ArgumentNullException("leaderboard");
            }
            
            this.gameData = gameData;
            this.leaderboard = leaderboard;
        }

        public void Execute(System.Collections.Generic.Dictionary<string, string> commandsOptionsValues)
        {
            var category = commandsOptionsValues["Category"];

            this.gameData.LevelCategory = category;
            this.leaderboard.LevelCategory = category;

            this.gameData.ExtractDataAsync(UnityEngine.Debug.LogException);
            this.leaderboard.LoadDataAsync();
            
            this.FinishedExecution = true;

            if (this.OnFinishedExecution != null)
            {
                this.OnFinishedExecution(this, EventArgs.Empty);
            }
        }
    }

}