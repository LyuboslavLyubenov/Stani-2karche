using System;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{

    using Assets.Scripts.Network;
    using Assets.Scripts.Utils;

    public class GameInfoFactory : MonoBehaviour
    {
        const string BasicExam = "BasicExam";
        const string AudienceRevenge = "AudienceRevenge";
        const string FastestWins = "FastestWins";

        public ServerNetworkManager ServerNetworkManager;
        public BasicExamServer BasicExamServer;

        string externalIp = "";

        void Awake()
        {
            NetworkUtils.GetExternalIP((ip) => this.externalIp = ip);
        }

        public CreatedGameInfo_Serializable Get()
        {
            var sceneName = SceneManager.GetActiveScene().name;
            return this.Get(sceneName);
        }

        public CreatedGameInfo_Serializable Get(string levelName)
        {
            var levelNameUpper = levelName.ToUpperInvariant();

            if (levelNameUpper.Contains(BasicExam.ToUpperInvariant()))
            {
                return this.GetBasicExamGameInfo();
            }
            else if (levelNameUpper.Contains(AudienceRevenge.ToUpperInvariant()))
            {
                //TODO
                throw new NotImplementedException();
            }
            else if (levelNameUpper.Contains(FastestWins.ToUpperInvariant()))
            {
                //TODO
                throw new NotImplementedException();
            }
            else
            {
                //TODO
                throw new NotImplementedException();
            }
        }

        BasicExamGameInfo_Serializable GetBasicExamGameInfo()
        {
            var canConnectAsMainPlayer = !this.BasicExamServer.MainPlayerData.IsConnected;
            var canConnectAsAudience = this.ServerNetworkManager.ConnectedClientsCount < (this.ServerNetworkManager.MaxConnections - 1);
            var gameType = GameType.BasicExam;
            var hostUsername = PlayerPrefsEncryptionUtils.HasKey("Username") ? PlayerPrefsEncryptionUtils.GetString("Username") : "Anonymous";
            var serverInfo = this.GetServerInfo();

            var gameInfo = new BasicExamGameInfo_Serializable()
                           {
                               CanConnectAsMainPlayer = canConnectAsMainPlayer,
                               CanConnectAsAudience = canConnectAsAudience,
                               GameType = gameType,
                               HostUsername = hostUsername,
                               ServerInfo = serverInfo
                           };

            return gameInfo;
        }

        ServerInfo_Serializable GetServerInfo()
        {
            var localIPAddress = NetworkUtils.GetLocalIP();
            var connectedClientsCount = this.ServerNetworkManager.ConnectedClientsCount;
            var maxConnections = this.ServerNetworkManager.MaxConnections;
            var serverInfo = new ServerInfo_Serializable()
                             {
                                 ExternalIpAddress = this.externalIp,
                                 LocalIPAddress = localIPAddress,
                                 ConnectedClientsCount = connectedClientsCount,
                                 MaxConnectionsAllowed = maxConnections
                             };

            return serverInfo;
        }
    }

}
