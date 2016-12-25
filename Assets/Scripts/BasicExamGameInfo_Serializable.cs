using System;

namespace Assets.Scripts
{

    [Serializable]
    public class BasicExamGameInfo_Serializable : CreatedGameInfo_Serializable
    {
        public bool CanConnectAsMainPlayer;
        public bool CanConnectAsAudience;
    }

}
