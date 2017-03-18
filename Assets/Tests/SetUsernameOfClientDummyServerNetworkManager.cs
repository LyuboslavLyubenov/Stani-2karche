namespace Tests
{

    using Interfaces.Network.NetworkManager;

    using Tests.DummyObjects;

    using Utils.Unity;

    using Zenject.Source.Usage;

    public class SetUsernameOfClientDummyServerNetworkManager : ExtendedMonoBehaviour
    {
        public string Username;

        public int ConnectionId;

        public float SetAfterTimeInSeconds = 1f;

        [Inject]
        private IServerNetworkManager networkManager;

        void Start()
        {
            this.CoroutineUtils.WaitForSeconds(this.SetAfterTimeInSeconds, this.SetUsername);
        }

        private void SetUsername()
        {
            ((DummyServerNetworkManager)this.networkManager).FakeSetUsernameToPlayer(this.ConnectionId, this.Username);
        }
    }

}