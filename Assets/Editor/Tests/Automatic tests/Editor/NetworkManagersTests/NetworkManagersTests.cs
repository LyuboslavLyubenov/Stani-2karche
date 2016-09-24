using UnityEngine;
using NUnit.Framework;

public class NetworkManagersTests
{
    ClientNetworkManager clientNetworkManager;
    ServerNetworkManager serverNetworkManager;

    [Test, Timeout(500)]
    public void ConnectClientToServer()
    {
        var obj = new GameObject();
        obj.name = "Client";
        clientNetworkManager = obj.AddComponent<ClientNetworkManager>();

        var obj2 = new GameObject();
        obj2.name = "Server";
        serverNetworkManager = obj2.AddComponent<ServerNetworkManager>();

        if (!serverNetworkManager.IsRunning)
        {
            serverNetworkManager.StartServer();
        }

        clientNetworkManager.ConnectToHost("127.0.0.1");
        Assert.IsTrue(clientNetworkManager.IsConnected);
    }
}
