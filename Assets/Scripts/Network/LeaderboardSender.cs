using UnityEngine;

namespace Assets.Scripts.Network
{

    using Assets.Scripts.Commands;
    using Assets.Scripts.Utils;

    public class LeaderboardSender : ExtendedMonoBehaviour
    {
        public ServerNetworkManager NetworkManager;
        public LeaderboardSerializer LeaderboardSerializer;



        void Start()
        {
            this.CoroutineUtils.WaitForFrames(0, () => this.Initialize());
        }

        void Initialize()
        {
            //tODO FIX TIMEOUT
            var sendLeaderboardEntitiesCommand = new DummyCommand();

            sendLeaderboardEntitiesCommand.OnExecuted += (sender, args) =>
                {
                    var connectionId = int.Parse(args.CommandsOptionsValues["ConnectionId"]);
                    this.StartSendingLeaderboardEntities(connectionId);
                };
        
            this.NetworkManager.CommandsManager.AddCommand("SendLeaderboardEntities", sendLeaderboardEntitiesCommand);   
        }

        void StartSendingLeaderboardEntities(int connectionId)
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