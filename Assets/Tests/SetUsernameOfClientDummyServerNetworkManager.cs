namespace Assets.Tests
{

    using Assets.Scripts.Interfaces.Network.NetworkManager;
    using Assets.Scripts.Utils.Unity;
    using Assets.Tests.DummyObjects;
    using Assets.Zenject.Source.Usage;

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