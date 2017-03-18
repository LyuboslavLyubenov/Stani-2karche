namespace DTOs
{

    using System;

    [Serializable]
    public class CreatedGameInfo_DTO
    {
        public ServerInfo_DTO ServerInfo;
        public string GameType;
        public string HostUsername;
    }

}
