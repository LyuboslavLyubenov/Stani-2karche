using NUnit.Framework;

using UnityEngine;

namespace Assets.Editor.Tests.Automatic_tests.Editor.NetworkManagersTests
{

    using Assets.Scripts.Network;
    using Assets.Scripts.Network.NetworkManagers;

    public class NetworkManagersTests
    {
        ClientNetworkManager clientNetworkManager;
        ServerNetworkManager serverNetworkManager;

        [Test, Timeout(500)]
        public void ConnectClientToServer()
        {
            var obj = new GameObject();
            obj.name = "Client";
            this.clientNetworkManager = obj.AddComponent<ClientNetworkManager>();

            var obj2 = new GameObject();
            obj2.name = "Server";
            this.serverNetworkManager = obj2.AddComponent<ServerNetworkManager>();

            if (!this.serverNetworkManager.IsRunning)
            {
                this.serverNetworkManager.StartServer();
            }

            this.clientNetworkManager.ConnectToHost("127.0.0.1");
            Assert.IsTrue(this.clientNetworkManager.IsConnected);
        }
    }

}
