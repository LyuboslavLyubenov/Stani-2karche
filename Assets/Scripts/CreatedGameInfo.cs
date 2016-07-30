using UnityEngine.UI;
using System;

public class CreatedGameInfo
{
    public ServerInfo ServerInfo
    {
        get;
        private set;
    }

    public GameType GameType
    {
        get;
        private set;
    }

    public string HostUsername
    {
        get;
        private set;
    }

    public CreatedGameInfo(ServerInfo server, GameType gameType, string hostUsername)
    {
        if (server == null)
        {
            throw new ArgumentNullException("server");
        }

        if (string.IsNullOrEmpty(hostUsername))
        {
            throw new ArgumentNullException("hostUsername");
        }

        this.ServerInfo = server;
        this.GameType = gameType;
        this.HostUsername = hostUsername;
    }
}
