namespace Assets.Scripts
{
    using System;

    using UnityEngine;
    using UnityEngine.SceneManagement;

    using DTOs;
    using Enums;
    using Network;
    using Utils.Unity;

    public class GameInfoFactory : MonoBehaviour
    {
        const string BasicExam = "BasicExam";

        public ServerNetworkManager ServerNetworkManager;
        public BasicExamServer BasicExamServer;

        string externalIp = "";

        void Awake()
        {
            NetworkUtils.GetExternalIP((ip) => this.externalIp = ip);
        }

        public CreatedGameInfo_DTO Get()
        {
            var sceneName = SceneManager.GetActiveScene().name;
            return this.Get(sceneName);
        }

        public CreatedGameInfo_DTO Get(string levelName)
        {
            var levelNameUpper = levelName.ToUpperInvariant();

            if (levelNameUpper.Contains(BasicExam.ToUpperInvariant()))
            {
                return this.GetBasicExamGameInfo();
            }

            throw new NotImplementedException();
        }

        BasicExamGameInfo_DTO GetBasicExamGameInfo()
        {
            var canConnectAsMainPlayer = !this.BasicExamServer.MainPlayerData.IsConnected;
            var canConnectAsAudience = this.ServerNetworkManager.ConnectedClientsCount < (this.ServerNetworkManager.MaxConnections - 1);
            var gameType = GameType.BasicExam;
            var hostUsername = PlayerPrefsEncryptionUtils.HasKey("Username") ? PlayerPrefsEncryptionUtils.GetString("Username") : "Anonymous";
            var serverInfo = this.GetServerInfo();

            var gameInfo = new BasicExamGameInfo_DTO()
                           {
                               CanConnectAsMainPlayer = canConnectAsMainPlayer,
                               CanConnectAsAudience = canConnectAsAudience,
                               GameType = gameType,
                               HostUsername = hostUsername,
                               ServerInfo = serverInfo
                           };

            return gameInfo;
        }

        ServerInfo_DTO GetServerInfo()
        {
            var localIPAddress = NetworkUtils.GetLocalIP();
            var connectedClientsCount = this.ServerNetworkManager.ConnectedClientsCount;
            var maxConnections = this.ServerNetworkManager.MaxConnections;
            var serverInfo = new ServerInfo_DTO()
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
