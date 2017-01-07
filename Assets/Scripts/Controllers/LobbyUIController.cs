namespace Assets.Scripts.Controllers
{

    using Assets.Scripts.Utils;

    using Network;
    using Network.Broadcast;
    using Network.TcpSockets;

    using UnityEngine;

    public class LobbyUIController : MonoBehaviour
    {
        public ConnectToExternalServerUIController ConnectToExternalServerUIController;
        public ServersAvailableUIController ServersAvailableUIController;

        private LANServersDiscoveryService LANServersDiscoveryService;

        private SimpleTcpServer tcpServer;

        private SimpleTcpClient tcpClient;

        private CreatedGameInfoReceiverService CreatedGameInfoReceiverService;

        void Awake()
        {
            var threadUtils = ThreadUtils.Instance;

            this.LANServersDiscoveryService = new LANServersDiscoveryService();
            this.tcpServer = new SimpleTcpServer(7772);
            this.tcpClient = new SimpleTcpClient();

            this.CreatedGameInfoReceiverService = new CreatedGameInfoReceiverService(this.tcpClient, this.tcpServer);
            
            this.ServersAvailableUIController.LANServersDiscoveryService = this.LANServersDiscoveryService;
            this.ServersAvailableUIController.GameInfoReceiverService = this.CreatedGameInfoReceiverService;
            this.ConnectToExternalServerUIController.GameInfoReceiverService = this.CreatedGameInfoReceiverService; 
        }
        
    }
}