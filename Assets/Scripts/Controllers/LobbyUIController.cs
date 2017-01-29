// ReSharper disable ArrangeTypeMemberModifiers
namespace Assets.Scripts.Controllers
{
    using Utils;

    using Network;
    using Network.Broadcast;
    using Network.TcpSockets;

    using UnityEngine;
    using UnityEngine.SceneManagement;

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

        void Start()
        {
            SceneManager.activeSceneChanged += OnActiveSceneChanged;
        }
    
        void OnApplicationQuit()
        {
            this.Dispose();
        }

        private void OnActiveSceneChanged(Scene oldScene, Scene newScene)
        {
            SceneManager.activeSceneChanged -= OnActiveSceneChanged;
            this.Dispose();
        }

        private void Dispose()
        {
            this.LANServersDiscoveryService.Dispose();
            this.tcpServer.Dispose();
            this.tcpClient.Dispose();

            this.tcpServer = null;
            this.tcpClient = null;
            this.LANServersDiscoveryService = null;

            this.CreatedGameInfoReceiverService = null;
        }
    }
}