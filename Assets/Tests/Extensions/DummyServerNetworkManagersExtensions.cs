using DummyServerNetworkManager = Tests.DummyObjects.DummyServerNetworkManager;
using MainPlayerConnectingCommand = Commands.Server.MainPlayerConnectingCommand;
using NetworkCommandData = Commands.NetworkCommandData;

namespace Assets.Tests.Extensions
{

    using Assets.Scripts.Commands.EveryBodyVsTheTeacher;

    public static class DummyServerNetworkManagersExtensions
    {

        public static void SimulateClientConnected(this DummyServerNetworkManager dummyNetworkManager, int connectionId, string username)
        {
            dummyNetworkManager.FakeConnectPlayer(connectionId);
            dummyNetworkManager.FakeSetUsernameToPlayer(connectionId, username);
        }
        
        public static void SimulateMainPlayerConnected(this DummyServerNetworkManager dummyNetworkManager, int connectionId, string username)
        {
            dummyNetworkManager.SimulateClientConnected(connectionId, username);
            var mainPlayerConnectingCommand = NetworkCommandData.From<MainPlayerConnectingCommand>();
            dummyNetworkManager.FakeReceiveMessage(connectionId, mainPlayerConnectingCommand.ToString());
        }

        public static void SimulatePresenterConnected(this DummyServerNetworkManager dummyNetworkManager, int connectionId)
        {
            dummyNetworkManager.SimulateClientConnected(connectionId, "Presenter " + connectionId);
            var presenterConnectingCommand = NetworkCommandData.From<PresenterConnectingCommand>();
            dummyNetworkManager.FakeReceiveMessage(connectionId, presenterConnectingCommand.ToString());
        }
    }
}
