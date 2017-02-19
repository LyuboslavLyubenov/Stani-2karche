namespace Assets.Tests.UI.Lobby.SelectPlayerTypeRouter
{

    using Assets.Scripts.DTOs;

    public class CreatedGameInfoCreatorForTests
    {
        private ServerInfo_DTO GenerateServerInfo()
        {
            var serverInfo = new ServerInfo_DTO()
            {
                ConnectedClientsCount = 2,
                MaxConnectionsAllowed = 10,
                LocalIPAddress = "127.0.0.1",
                ExternalIpAddress = "127.0.0.1"
            };

            return serverInfo;
        }

        public BasicExamGameInfo_DTO GenerateBasicExamGameInfo()
        {
            var serverInfo = this.GenerateServerInfo();
            var basicExamGameType = new BasicExamGameInfo_DTO()
            {
                CanConnectAsAudience = true,
                CanConnectAsMainPlayer = true,
                GameType = "BasicExam",
                HostUsername = "Ivan",
                ServerInfo = serverInfo
            };
            return basicExamGameType;
        }

        public EveryBodyVsTheTeacherGameInfo_DTO GenerateEveryBodyVsTheTeacherGameInfo()
        {
            var serverInfo = this.GenerateServerInfo();
            var everybodyVsTheTeacher = new EveryBodyVsTheTeacherGameInfo_DTO()
            {
                CanConnectAsAudience = true,
                CanConnectAsMainPlayer = true,
                GameType = "EveryBodyVsTheTeacher",
                HostUsername = "Ivan",
                ServerInfo = serverInfo,
                IsGameStarted = false
            };

            return everybodyVsTheTeacher;
        }
    }

}