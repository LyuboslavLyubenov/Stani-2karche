namespace Assets.Scripts.Network.Leaderboard
{
    using System;

    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Interfaces.Network;

    using Commands.Client;

    using EventArgs;

    using Commands;
    using DTOs;
    using IO;

    using NetworkManagers;

    using UnityEngine;

    public class LeaderboardSender : ILeaderboardSender
    {
        public event EventHandler<LeaderboardDataEventArgs> OnSentLeaderboard = delegate
            { };

        private readonly IServerNetworkManager networkManager;
        private readonly ILeaderboardDataManipulator leaderboardDataManipulator;

        public LeaderboardSender(IServerNetworkManager networkManager, ILeaderboardDataManipulator leaderboardDataManipulator)
        {
            if (networkManager == null)
            {
                throw new ArgumentNullException("networkManager");
            }

            if (leaderboardDataManipulator == null)
            {
                throw new ArgumentNullException("leaderboardDataManipulator");
            }
            
            this.networkManager = networkManager;
            this.leaderboardDataManipulator = leaderboardDataManipulator;

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
            var allPlayersData = this.leaderboardDataManipulator.Leaderboard;

            for (int i = 0; i < allPlayersData.Count; i++)
            {
                var playerScore = allPlayersData[i];
                var playerScoreSer = new PlayerScore_Dto(playerScore);
                var json = JsonUtility.ToJson(playerScoreSer);
                var sendEntityCommand = NetworkCommandData.From<LeaderboardEntityCommand>();
                sendEntityCommand.AddOption("PlayerScoreJSON", json);

                this.networkManager.SendClientCommand(connectionId, sendEntityCommand);
            }

            var noMoreEntitiesCommand = new NetworkCommandData("LeaderboardNoMoreEntities");
            this.networkManager.SendClientCommand(connectionId, noMoreEntitiesCommand);

            OnSentLeaderboard(this, new LeaderboardDataEventArgs(allPlayersData));
        }
    }
}