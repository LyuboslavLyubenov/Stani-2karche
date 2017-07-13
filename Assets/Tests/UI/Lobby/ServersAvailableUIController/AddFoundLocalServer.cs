using CreatedGameInfoSender = Network.GameInfo.CreatedGameInfoSender;

namespace Tests.UI.Lobby.ServersAvailableUIController
{

    using Controllers;

    using DTOs;

    using Interfaces.Network;
    using Interfaces.Services;

    using Tests.DummyObjects;

    using UnityEngine;

    using Utils.Unity;

    using Zenject;

    public class AddFoundLocalServer : ExtendedMonoBehaviour
    {
        public ServersAvailableUIController ServersAvailableUiController;

        public float StartAfterTimeInSeconds = 1f;

        public string GameType;

        public string HostUsername;

        public int ConnectedClientsCount;
        public int MaxConnections;
        public string ExternalIP;
        public string LocalIP;
        
        [Inject]
        private ILANServersDiscoverer IlanServersDiscoverer;

        [Inject]
        private ISimpleTcpServer tcpServer;

        void Start()
        {
            this.CoroutineUtils.WaitForSeconds(this.StartAfterTimeInSeconds, this.AddServer);
        }

        private void AddServer()
        {
            this.FakeFoundLocalServer();
            this.CoroutineUtils.WaitForSeconds(0.5f, this.SendGameInfo);
        }

        private void FakeFoundLocalServer()
        {
            var service = (DummyIlanServersDiscoverer)this.IlanServersDiscoverer;
            service.FakeFoundServer(this.LocalIP);
        }

        private string GenerateGameInfoJson()
        {
            var serverInfo = new ServerInfo_DTO()
            {
                ConnectedClientsCount = this.ConnectedClientsCount,
                ExternalIpAddress = this.ExternalIP,
                LocalIPAddress = this.LocalIP,
                MaxConnectionsAllowed = this.MaxConnections
            };
            var createdGameInfo = new CreatedGameInfo_DTO()
            {
                GameType = this.GameType,
                HostUsername = this.HostUsername,
                ServerInfo = serverInfo
            };
            var json = JsonUtility.ToJson(createdGameInfo);
            return json;
        }

        private void SendGameInfo()
        {
            var dummyTcpServer = (DummySimpleTcpServer)this.tcpServer;
            var json = this.GenerateGameInfoJson();

            dummyTcpServer.FakeConnectClient("127.0.0.1");
            dummyTcpServer.FakeReceiveMessageFromClient("127.0.0.1", CreatedGameInfoSender.GameInfoTag + json);
        }
    }
}
