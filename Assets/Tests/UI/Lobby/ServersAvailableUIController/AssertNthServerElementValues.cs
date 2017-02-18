namespace Assets.Tests.UI.Lobby.ServersAvailableUIController
{

    using Assets.Scripts.Controllers;
    using Assets.Scripts.Utils.Unity;

    using UnityEngine;

    public class AssertNthServerElementValues : ExtendedMonoBehaviour
    {
        public int ElementIndex = 0;

        public string ExpectedGameType;
        public string ExpectedCreatorName;
        public string ExpectedConnectedClientsCount;

        public float AfterTimeInSeconds = 0f;

        // Use this for initialization
        void Start ()
        {
		    this.CoroutineUtils.WaitForSeconds(this.AfterTimeInSeconds, Validate);
        }

        private void Validate()
        {
            var serversAvailableUIController = GameObject.FindObjectOfType<ServersAvailableUIController>();
            var servers = serversAvailableUIController.Container.GetComponentsInChildren<ServerDiscoveredElementController>();
            var server = servers[this.ElementIndex];
            var isWithExpectedData = server.GameType == this.ExpectedGameType
                                     && server.CreatorName == this.ExpectedCreatorName
                                     && server.ConnectedClients == this.ExpectedConnectedClientsCount;

            if (isWithExpectedData)
            {
                IntegrationTest.Pass();
            }
            else
            {
                IntegrationTest.Fail();
            }
        }
	
    }

}
