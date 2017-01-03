namespace Assets.Scripts.Network.Leaderboard
{
    using System;

    using EventArgs;

    using Commands;
    using DTOs;
    using IO;

    using NetworkManagers;

    using UnityEngine;

    public class LeaderboardSender
    {
        public event EventHandler<LeaderboardDataEventArgs> OnSentLeaderboard = delegate
            { };

        private readonly ServerNetworkManager networkManager;
        private readonly LeaderboardSerializer leaderboardSerializer;

        public LeaderboardSender(ServerNetworkManager networkManager, LeaderboardSerializer leaderboardSerializer)
        {
            if (networkManager == null)
            {
                throw new ArgumentNullException("networkManager");
            }

            if (leaderboardSerializer == null)
            {
                throw new ArgumentNullException("leaderboardSerializer");
            }
            
            this.networkManager = networkManager;
            this.leaderboardSerializer = leaderboardSerializer;

            var sendLeaderboardEntitiesCommand = new DummyCommand();

            sendLeaderboardEntitiesCommand.OnExecuted += (sender, args) =>
            {
                var connectionId = int.Parse(args.CommandsOptionsValues["ConnectionId"]);
                this.StartSendingLeaderboardEntities(connectionId);
            };

            this.networkManager.CommandsManager.AddCommand("SendLeaderboardEntities", sendLeaderboardEntitiesCommand);
        }
        

        private void StartSendingLeaderboardEntities(int connectionId)
        {
            var allPlayersData = this.leaderboardSerializer.Leaderboard;

            for (int i = 0; i < allPlayersData.Count; i++)
            {
                var playerScore = allPlayersData[i];
                var playerScoreSer = new PlayerScore_Serializable(playerScore);
                var json = JsonUtility.ToJson(playerScoreSer);
                var sendEntityCommand = new NetworkCommandData("LeaderboardEntity");
                sendEntityCommand.AddOption("PlayerScoreJSON", json);

                this.networkManager.SendClientCommand(connectionId, sendEntityCommand);
            }

            var noMoreEntitiesCommand = new NetworkCommandData("LeaderboardNoMoreEntities");
            this.networkManager.SendClientCommand(connectionId, noMoreEntitiesCommand);

            OnSentLeaderboard(this, new LeaderboardDataEventArgs(allPlayersData));
        }
    }
}