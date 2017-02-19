// ReSharper disable ArrangeTypeMemberModifiers
namespace Assets.Scripts.Controllers.Lobby
{

    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Network;
    using Assets.Scripts.Network.Broadcast;
    using Assets.Scripts.Utils;

    using UnityEngine;
    using UnityEngine.SceneManagement;

    using Zenject;

    public class LobbyUIController : MonoBehaviour
    {
        [Inject]
        private ConnectToExternalServerUIController connectToExternalServerUIController;

        [Inject]
        private ServersAvailableUIController serversAvailableUIController;

        [Inject]
        private ILANServersDiscoveryService LANServersDiscoveryService;
        
        [Inject]
        private CreatedGameInfoReceiverService createdGameInfoReceiverService;

        [Inject]
        private ISimpleTcpClient tcpClient;

        [Inject]
        private ISimpleTcpServer tcpServer;
        
        void Awake()
        {
            var threadUtils = ThreadUtils.Instance;
        }

        void Start()
        {
            SceneManager.activeSceneChanged += this.OnActiveSceneChanged;
        }
    
        void OnApplicationQuit()
        {
            this.Dispose();
        }

        private void OnActiveSceneChanged(Scene oldScene, Scene newScene)
        {
            SceneManager.activeSceneChanged -= this.OnActiveSceneChanged;
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

            this.createdGameInfoReceiverService = null;
        }
    }
}