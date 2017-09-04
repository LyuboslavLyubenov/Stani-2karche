using System;
using System.Linq;
using System.Reflection;

using DTOs;

using Extensions;

using Interfaces;
using Interfaces.Network.NetworkManager;

using Network.Servers;
using Network.Servers.EveryBodyVsTheTeacher;

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

    public CreatedGameInfo_DTO Get(IServerNetworkManager serverNetworkManager, IGameServer server)
    {
        var gameName = server.GetGameTypeName();
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

    private BasicExamGameInfo_DTO GetBasicExamGameInfo(IServerNetworkManager serverNetworkManager, IGameServer server)
    {
        var canConnectAsMainPlayer = !((BasicExamServer)server).MainPlayerData.IsConnected;
        var canConnectAsAudience = serverNetworkManager.ConnectedClientsCount < (serverNetworkManager.MaxConnections - 1);
        var gameTypeName = server.GetGameTypeName();
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

    private EveryBodyVsTheTeacherGameInfo_DTO GetEveryBodyVsTheTeacherGameInfo(IServerNetworkManager networkManager, IGameServer gameServer)
    {
        var server = (EveryBodyVsTheTeacherServer)gameServer;

        var canConnectAsMainPlayer = 
            !server.IsGameOver &&
            !server.StartedGame &&
            server.ConnectedMainPlayersConnectionIds.Count() < EveryBodyVsTheTeacherServer.MaxMainPlayersNeededToStartGame;

        var canConnectAsAudience = 
            !server.IsGameOver &&
            !server.StartedGame;

        var gameTypeName = server.GetGameTypeName();
        var hostUsername = this.GetHostUsername();
        var serverInfo = this.GetServerInfo(networkManager);

        var gameInfo = new EveryBodyVsTheTeacherGameInfo_DTO()
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
        
    private string GetHostUsername()
    {
        return PlayerPrefsEncryptionUtils.HasKey("Username") ? PlayerPrefsEncryptionUtils.GetString("Username") : "Anonymous";
    }

    private ServerInfo_DTO GetServerInfo(IServerNetworkManager serverNetworkManager)
    {
        var localIPAddress = NetworkUtils.GetLocalIP();
        var connectedClientsCount = serverNetworkManager.ConnectedClientsCount - 1;
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