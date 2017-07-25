// ReSharper disable ArrangeTypeMemberModifiers
namespace Controllers.Lobby
{
    using Interfaces.Services;

    using UnityEngine.SceneManagement;

    using Utils;
    using Utils.Unity;

    using Zenject;

    public class LobbyUIController : ExtendedMonoBehaviour
    {
        [Inject]
        private ILANServersDiscoverer IlanServersDiscoverer;
        
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
        }
    }
}