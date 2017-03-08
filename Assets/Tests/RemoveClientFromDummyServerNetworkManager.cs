namespace Assets.Tests
{

    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Interfaces.Network.NetworkManager;
    using Assets.Scripts.Utils.Unity;
    using Assets.Zenject.Source.Usage;

    public class RemoveClientFromDummyServerNetworkManager : ExtendedMonoBehaviour
    {
        public int ConnectionId = 1;

        public float DisconnectAfterTimeInSeconds = 1f;

        [Inject]
        private IServerNetworkManager networkManager;

        void Start()
        {
            this.CoroutineUtils.WaitForSeconds(this.DisconnectAfterTimeInSeconds, this.DisconnectClientFromServer);
        }

        void DisconnectClientFromServer()
        {
            ((DummyServerNetworkManager)this.networkManager).FakeDisconnectPlayer(this.ConnectionId);
        }
    }
}