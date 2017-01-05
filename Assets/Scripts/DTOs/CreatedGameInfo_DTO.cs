namespace Assets.Scripts.DTOs
{
    using System;

    [Serializable]
    public class CreatedGameInfo_DTO
    {
        public ServerInfo_DTO ServerInfo;
        public string GameTypeFullName;
        public string HostUsername;
    }

}
