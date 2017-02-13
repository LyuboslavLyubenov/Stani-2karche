namespace Assets.Scripts
{
    using System;
    using System.Linq;
    using System.Reflection;

    using Assets.Scripts.Controllers.GameController;
    using Assets.Scripts.Interfaces;

    using Network.NetworkManagers;
    using Network.Servers;

    using DTOs;

    using Utils.Unity;

    public class GameInfoFactory
    {
        private static GameInfoFactory instance;

        public static GameInfoFactory Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GameInfoFactory();
                }

                return instance;
            }
        }

        private string externalIp = "";

        GameInfoFactory()
        {
            NetworkUtils.GetExternalIP((ip) => this.externalIp = ip);
        }

        public CreatedGameInfo_DTO Get(ServerNetworkManager serverNetworkManager, IGameServer server)
        {
            var gameName = server.GetType()
                .Name.Replace("Server", "");
            var methodName = "Get" + gameName + "GameInfo";
            var methodInfo = this.GetType()
                .GetMethod(
                    methodName,
                    BindingFlags.NonPublic |
                    BindingFlags.Instance |
                    BindingFlags.InvokeMethod);

            if (methodInfo == null)
            {
                throw new NotImplementedException();
            }

            return (CreatedGameInfo_DTO)methodInfo.Invoke(this, new object[] { serverNetworkManager, server });
        }

        private BasicExamGameInfo_DTO GetBasicExamGameInfo(ServerNetworkManager serverNetworkManager, IGameServer server)
        {
            var canConnectAsMainPlayer = !((BasicExamServer)server).MainPlayerData.IsConnected;
            var canConnectAsAudience = serverNetworkManager.ConnectedClientsCount < (serverNetworkManager.MaxConnections - 1);
            var gameTypeName = this.GetServerTypeName(server);
            var hostUsername = this. GetHostUsername();
            var serverInfo = this.GetServerInfo(serverNetworkManager);

            var gameInfo = new BasicExamGameInfo_DTO()
            {
                CanConnectAsMainPlayer = canConnectAsMainPlayer,
                CanConnectAsAudience = canConnectAsAudience,
                GameType = gameTypeName,
                HostUsername = hostUsername,
                ServerInfo = serverInfo
            };

            return gameInfo;
        }

        private EverybodyVsTheTeacherGameInfo_DTO GetEveryBodyVsTheTeacherGameInfo(ServerNetworkManager networkManager, IGameServer gameServer)
        {
            var server = (EveryBodyVsTheTeacherServer)gameServer;

            var canConnectAsMainPlayer = 
                !server.IsGameOver &&
                !server.StartedGame &&
                server.MainPlayersConnectionIds.Count() < EveryBodyVsTheTeacherServer.MaxMainPlayersNeededToStartGame;

            var canConnectAsAudience = 
                !server.IsGameOver &&
                !server.StartedGame;

            var gameTypeName = this.GetServerTypeName(server);
            var hostUsername = this.GetHostUsername();
            var serverInfo = this.GetServerInfo(networkManager);

            var gameInfo = new EverybodyVsTheTeacherGameInfo_DTO()
            {
                GameType = gameTypeName,
                HostUsername = hostUsername,
                ServerInfo = serverInfo,
                CanConnectAsMainPlayer = canConnectAsMainPlayer,
                CanConnectAsAudience = canConnectAsAudience,
                IsGameStarted = server.StartedGame
            };

            return gameInfo;
        }

        private string GetServerTypeName(IGameServer server)
        {
            return server.GetType()
                .Name.Replace("Server", "");
        }

        private string GetHostUsername()
        {
            return PlayerPrefsEncryptionUtils.HasKey("Username") ? PlayerPrefsEncryptionUtils.GetString("Username") : "Anonymous";
        }

        private ServerInfo_DTO GetServerInfo(ServerNetworkManager serverNetworkManager)
        {
            var localIPAddress = NetworkUtils.GetLocalIP();
            var connectedClientsCount = serverNetworkManager.ConnectedClientsCount;
            var maxConnections = serverNetworkManager.MaxConnections;
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