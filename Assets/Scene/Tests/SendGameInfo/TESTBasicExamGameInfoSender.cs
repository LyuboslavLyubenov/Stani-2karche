using UnityEngine;

public class TESTBasicExamGameInfoSender : ExtendedMonoBehaviour
{
    public CreatedGameInfoSenderService Sender;

    void Start()
    {
        CoroutineUtils.WaitForSeconds(1, TESTSend);
    }

    void TESTSend()
    {
        var serverInfo = new ServerInfo_Serializable()
        {
            IPAddress = "127.0.0.1",
            ConnectedClients = 0,
            MaxConnectionsAllowed = 10
        };
        
        var gameInfo = new BasicExamGameInfo_Serializable()
        {
            ServerInfo = serverInfo,
            GameType = GameType.BasicExam,
            HostUsername = "Dead4y",
            CanConnectAsAudience = true,
            CanConnectAsMainPlayer = false
        };
        
        Sender.SendGameInfo("192.168.0.101", (CreatedGameInfo_Serializable)gameInfo, delegate
            {
            });
    }
}
