using UnityEngine;

public class TEST_UploadServerInfo : ExtendedMonoBehaviour
{
    void Start()
    {
        CoroutineUtils.WaitForSeconds(1, TestUploadGameInfo);
    }

    void TestUploadGameInfo()
    {
        KinveyWrapper.Instance.LoginAsync("ivan", "ivan", (data) =>
            {
                var serverInfo = new ServerInfo_Serializable()
                {
                    ExternalIpAddress = "192.168.0.1", 
                    LocalIPAddress = "127.0.0.1",
                    ConnectedClientsCount = 1,
                    MaxConnectionsAllowed = 30
                };
                var gameInfo = new CreatedGameInfo_Serializable()
                { 
                    ServerInfo = serverInfo,
                    GameType = GameType.BasicExam,
                    HostUsername = "ivan gotiniq"
                };
                
                KinveyWrapper.Instance.CreateEntityAsync<CreatedGameInfo_Serializable>("Servers", gameInfo, () =>
                    {
                        Debug.Log("Successfully uploaded info");
                    }, Debug.LogException); 
            }, Debug.LogException);
    }
	
}
