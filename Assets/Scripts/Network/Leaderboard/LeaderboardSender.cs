namespace Assets.Scripts.Network.Leaderboard
{

    using Commands;
    using DTOs;
    using IO;
    using NetworkManagers;
    using Utils.Unity;

    using UnityEngine;

    public class LeaderboardSender : ExtendedMonoBehaviour
    {
        public ServerNetworkManager NetworkManager;
        public LeaderboardSerializer LeaderboardSerializer;

        // ReSharper disable once ArrangeTypeMemberModifiers
        void Start()
        {
            this.CoroutineUtils.WaitForFrames(0, this.Initialize);
        }

        private void Initialize()
        {
            var sendLeaderboardEntitiesCommand = new DummyCommand();

            sendLeaderboardEntitiesCommand.OnExecuted += (sender, args) =>
                {
                    var connectionId = int.Parse(args.CommandsOptionsValues["ConnectionId"]);
                    this.StartSendingLeaderboardEntities(connectionId);
                };
        
            this.NetworkManager.CommandsManager.AddCommand("SendLeaderboardEntities", sendLeaderboardEntitiesCommand);   
        }

        private void StartSendingLeaderboardEntities(int connectionId)
        {
            var allPlayersData = this.LeaderboardSerializer.Leaderboard;

            for (int i = 0; i < allPlayersData.Count; i++)
            {
                var playerScore = allPlayersData[i];
                var playerScoreSer = new PlayerScore_Serializable(playerScore);
                var json = JsonUtility.ToJson(playerScoreSer);
                var sendEntityCommand = new NetworkCommandData("LeaderboardEntity");
                sendEntityCommand.AddOption("PlayerScoreJSON", json);

                this.NetworkManager.SendClientCommand(connectionId, sendEntityCommand);
            }

            var noMoreEntitiesCommand = new NetworkCommandData("LeaderboardNoMoreEntities");
            this.NetworkManager.SendClientCommand(connectionId, noMoreEntitiesCommand);
        }

    }

}