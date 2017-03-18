
using BasicExamServer = Network.Servers.BasicExamServer;
using KinveyWrapper = Network.KinveyWrapper;

namespace Tests.Test_Kinvey_Wrapper.Upload_and_retrieve_from_collection
{

    using DTOs;
    using DTOs.KinveyDtoObjs;

    using UnityEngine;

    using Utils.Unity;

    public class TEST_UploadServerInfo : ExtendedMonoBehaviour
    {
        private void Start()
        {
            this.CoroutineUtils.WaitForSeconds(1, this.TestUploadGameInfo);
        }

        private void TestUploadGameInfo()
        {
            var kinveyWrapper = new KinveyWrapper();

            kinveyWrapper.LoginAsync(
                "ivan", 
                "ivan", 
                (data) => this.OnReceivedLoginResponse(kinveyWrapper, data), 
                Debug.LogException);
        }

        private void OnReceivedLoginResponse(KinveyWrapper kinveyWrapper, _UserReceivedData data)
        {
            var serverInfo = new ServerInfo_DTO()
            {
                ExternalIpAddress = "192.168.0.1",
                LocalIPAddress = "127.0.0.1",
                ConnectedClientsCount = 1,
                MaxConnectionsAllowed = 30
            };
            var gameInfo = new CreatedGameInfo_DTO()
            {
                ServerInfo = serverInfo,
                GameType = typeof(BasicExamServer).FullName,
                HostUsername = "ivan gotiniq"
            };

            kinveyWrapper.CreateEntityAsync(
                "Servers",
                gameInfo,
                () => Debug.Log("Successfully uploaded info"),
                Debug.LogException);
        }
	
    }

}
