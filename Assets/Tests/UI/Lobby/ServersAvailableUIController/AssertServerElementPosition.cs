namespace Assets.Tests.UI.Lobby.ServersAvailableUIController
{

    using Assets.Scripts.Controllers;
    using Assets.Scripts.Controllers.Lobby;
    using Assets.Scripts.Utils.Unity;
    using Assets.UnityTestTools.IntegrationTestsFramework.TestRunner;

    using UnityEngine;

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
            var isOnTargetPosition = rect.Overlaps(Target.rect);

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