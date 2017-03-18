// ReSharper disable ArrangeTypeMemberModifiers
namespace Controllers.Lobby
{

    using Interfaces.Network;
    using Interfaces.Services;

    using Network.GameInfo;

    using UnityEngine;
    using UnityEngine.SceneManagement;

    using Utils;

    using Zenject.Source.Usage;

    public class LobbyUIController : MonoBehaviour
    {
        [Inject]
        private ConnectToExternalServerUIController connectToExternalServerUIController;

        [Inject]
        private ServersAvailableUIController serversAvailableUIController;

        [Inject]
        private ILANServersDiscoverer IlanServersDiscoverer;
        
        [Inject]
        private CreatedGameInfoReceiver createdGameInfoReceiver;

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
            this.IlanServersDiscoverer.Dispose();
            this.tcpServer.Dispose();
            this.tcpClient.Dispose();

            this.tcpServer = null;
            this.tcpClient = null;
            this.IlanServersDiscoverer = null;

            this.createdGameInfoReceiver = null;
        }
    }
}