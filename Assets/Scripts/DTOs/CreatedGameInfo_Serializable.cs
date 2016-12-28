namespace Assets.Scripts.DTOs
{

    using System;

    using Assets.Scripts.Enums;
    using Assets.Scripts.Network;

    [Serializable]
    public class CreatedGameInfo_Serializable
    {
        public ServerInfo_Serializable ServerInfo;
        public GameType GameType;
        public string HostUsername;
    }

}
