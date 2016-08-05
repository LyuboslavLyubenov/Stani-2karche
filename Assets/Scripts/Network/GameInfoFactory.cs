using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class GameInfoFactory : MonoBehaviour
{
    public ServerNetworkManager ServerNetworkManager;

    public CreatedGameInfo_Serializable Get()
    {
        var sceneName = SceneManager.GetActiveScene().name;

        if (sceneName.Contains("BasicExam"))
        {
            return GetBasicExamGameInfo();
        }
        else if (sceneName.Contains("AudienceRevenge"))
        {
            //TODO
            throw new NotImplementedException();
        }
        else if (sceneName.Contains("FastestsWins"))
        {
            //TODO
            throw new NotImplementedException();
        }

        return null;
    }

    BasicExamGameInfo_Serializable GetBasicExamGameInfo()
    {
        var canConnectAsMainPlayer = PlayerPrefs.HasKey("CanConnectAsMainPlayer");
        var canConnectAsAudience = PlayerPrefs.HasKey("CanConnectAsAudience");
        var gameType = GameType.BasicExam;
        var hostUsername = PlayerPrefs.GetString("Username");
        var serverInfo = GetServerInfo();

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
        //TODO get external ip address
        var externalIpAddress = "";
        var localIPAddress = NetworkUtils.GetLocalIP();
        var connectedClientsCount = ServerNetworkManager.ConnectedClientsCount;
        var maxConnections = ServerNetworkManager.MaxConnections;
         
        var serverInfo = new ServerInfo_Serializable()
        {
            ExternalIpAddress = externalIpAddress,
            LocalIPAddress = localIPAddress,
            ConnectedClientsCount = connectedClientsCount,
            MaxConnectionsAllowed = maxConnections
        };

        return serverInfo;
    }
}
