namespace Assets.Scripts.DTOs
{

    using System;

    [Serializable]
    public class EverybodyVsTheTeacherGameInfo_DTO : CreatedGameInfo_DTO
    {
        public bool CanConnectAsMainPlayer;
        public bool CanConnectAsAudience;
        public bool IsGameStarted;
    }

}