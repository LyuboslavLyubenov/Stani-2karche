namespace DTOs
{

    using System;

    [Serializable]
    public class BasicExamGameInfo_DTO : CreatedGameInfo_DTO
    {
        public bool CanConnectAsMainPlayer;
        public bool CanConnectAsAudience;
    }
}
