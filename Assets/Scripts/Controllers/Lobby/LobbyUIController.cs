// ReSharper disable ArrangeTypeMemberModifiers
namespace Assets.Scripts.Controllers.Lobby
{
    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Network;
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