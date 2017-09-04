namespace Commands.Server
{
    using System;

    using Interfaces;
    using Interfaces.GameData;
    using Interfaces.Network.NetworkManager;

    using Notifications;

    using UnityEngine;

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

        private void OnGameDataExtractError(Exception exception)
        {
            NotificationsController.Instance.AddNotification(Color.red, exception.Message, 20);
        }

        public void Execute(System.Collections.Generic.Dictionary<string, string> commandsOptionsValues)
        {
            var category = commandsOptionsValues["Category"];

            this.gameData.LevelCategory = category;
            this.leaderboard.LevelCategory = category;

            this.gameData.ExtractDataAsync(this.OnGameDataExtractError);
            this.leaderboard.LoadDataAsync();
            
            this.FinishedExecution = true;

            if (this.OnFinishedExecution != null)
            {
                this.OnFinishedExecution(this, EventArgs.Empty);
            }
        }
    }

}