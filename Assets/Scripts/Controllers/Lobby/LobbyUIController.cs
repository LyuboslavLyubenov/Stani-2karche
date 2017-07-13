// ReSharper disable ArrangeTypeMemberModifiers
namespace Controllers.Lobby
{
    using Interfaces.Network.NetworkManager;
    using Interfaces.Services;
    
    using UnityEngine;
    using UnityEngine.SceneManagement;

    using Utils;

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
        private ICreatedGameInfoReceiver createdGameInfoReceiver;
        
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
            
            this.IlanServersDiscoverer = null;

            this.createdGameInfoReceiver = null;
        }
    }
}