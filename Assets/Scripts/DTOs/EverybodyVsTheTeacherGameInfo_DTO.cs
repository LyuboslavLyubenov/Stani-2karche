namespace DTOs
{

    using System;

    [Serializable]
    public class EveryBodyVsTheTeacherGameInfo_DTO : CreatedGameInfo_DTO
    {
        public bool CanConnectAsMainPlayer;
        public bool CanConnectAsAudience;
        public bool IsGameStarted;
    }

}