using UnityEngine;
using System;
using UnityEngine.SceneManagement;

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
        NetworkUtils.GetExternalIP((ip) => externalIp = ip);
    }

    public CreatedGameInfo_Serializable Get()
    {
        var sceneName = SceneManager.GetActiveScene().name;
        return Get(sceneName);
    }

    public CreatedGameInfo_Serializable Get(string levelName)
    {
        var levelNameUpper = levelName.ToUpperInvariant();

        if (levelNameUpper.Contains(BasicExam.ToUpperInvariant()))
        {
            return GetBasicExamGameInfo();
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
            throw new NotImplementedException();
        }
    }

    BasicExamGameInfo_Serializable GetBasicExamGameInfo()
    {
        var canConnectAsMainPlayer = !BasicExamServer.MainPlayerData.IsConnected;
        var canConnectAsAudience = ServerNetworkManager.ConnectedClientsCount < (ServerNetworkManager.MaxConnections - 1);
        var gameType = GameType.BasicExam;
        var hostUsername = PlayerPrefsEncryptionUtils.GetString("Username");
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
        var localIPAddress = NetworkUtils.GetLocalIP();
        var connectedClientsCount = ServerNetworkManager.ConnectedClientsCount;
        var maxConnections = ServerNetworkManager.MaxConnections;
        var serverInfo = new ServerInfo_Serializable()
        {
            ExternalIpAddress = externalIp,
            LocalIPAddress = localIPAddress,
            ConnectedClientsCount = connectedClientsCount,
            MaxConnectionsAllowed = maxConnections
        };

        return serverInfo;
    }
}
