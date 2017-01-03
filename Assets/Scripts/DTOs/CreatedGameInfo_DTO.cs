namespace Assets.Scripts.DTOs
{
    using System;

    using Enums;
    using Network;

    [Serializable]
    public class CreatedGameInfo_DTO
    {
        public ServerInfo_DTO ServerInfo;
        public string GameTypeFullName;
        public string HostUsername;
    }

}
