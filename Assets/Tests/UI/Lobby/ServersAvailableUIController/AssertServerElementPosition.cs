namespace Tests.UI.Lobby.ServersAvailableUIController
{

    using Controllers;
    using Controllers.Lobby;

    using UnityEngine;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Utils.Unity;

    public class AssertServerElementPosition : ExtendedMonoBehaviour
    {
        public int ElementIndex = 0;
        public RectTransform Target;
        public float AssertAfterTimeInSeconds = 1f;

        void Start()
        {
            this.CoroutineUtils.WaitForSeconds(this.AssertAfterTimeInSeconds, this.AssertPosition);
        }

        private void AssertPosition()
        {
            var serversAvailableUIController = GameObject.FindObjectOfType<ServersAvailableUIController>();
            var servers = serversAvailableUIController.Container.GetComponentsInChildren<ServerDiscoveredElementController>();
            var server = servers[this.ElementIndex];
            var rect = server.GetComponent<RectTransform>()
                .rect;
            var isOnTargetPosition = rect.Overlaps(this.Target.rect);

            if (isOnTargetPosition)
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