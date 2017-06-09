using DummyServerNetworkManager = Tests.DummyObjects.DummyServerNetworkManager;
using MainPlayerConnectingCommand = Commands.Server.MainPlayerConnectingCommand;
using NetworkCommandData = Commands.NetworkCommandData;

namespace Assets.Tests.Extensions
{

    public static class DummyServerNetworkManagersExtensions
    {

        public static void SimulateClientConnected(this DummyServerNetworkManager networkManager, int connectionId, string username)
        {
            networkManager.FakeConnectPlayer(connectionId);
            networkManager.FakeSetUsernameToPlayer(connectionId, username);
        }
        
        public static void SimulateMainPlayerConnected(this DummyServerNetworkManager dummyServerNetworkManager, int connectionId, string username)
        {
            dummyServerNetworkManager.SimulateClientConnected(connectionId, username);
            var mainPlayerConnectingCommand = NetworkCommandData.From<MainPlayerConnectingCommand>();
            dummyServerNetworkManager.FakeReceiveMessage(connectionId, mainPlayerConnectingCommand.ToString());
        }
    }
}
