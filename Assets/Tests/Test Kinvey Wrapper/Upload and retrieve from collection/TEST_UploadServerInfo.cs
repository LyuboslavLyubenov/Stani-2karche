
namespace Assets.Tests.Test_Kinvey_Wrapper.Upload_and_retrieve_from_collection
{

    using Assets.Scripts.DTOs.KinveySerializableObj;
    using Assets.Scripts.Network.Servers;

    using UnityEngine;

    using Scripts.DTOs;
    using Scripts.Network;
    using Scripts.Utils.Unity;

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
                (data) => OnReceivedLoginResponse(kinveyWrapper, data), 
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
                GameTypeFullName = typeof(BasicExamServer).FullName,
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
