namespace Assets.Tests.UI.EverybodyVsTheTeacher.MainPlayersContainer
{

    using Assets.Scripts.Commands;
    using Assets.Scripts.Interfaces.Network.NetworkManager;
    using Assets.Scripts.Utils.Unity;
    using Assets.Zenject.Source.Usage;
    
    public class RequestGameStart : ExtendedMonoBehaviour
    {
        [Inject]
        private IServerNetworkManager networkManager;

        public int ConnectionId = 1;

        public float AfterTimeInSeconds = 1f;

        void Start()
        {
            this.CoroutineUtils.WaitForSeconds(this.AfterTimeInSeconds, this.SendRequestGameStartCommand);
        }

        private void SendRequestGameStartCommand()
        {
            var networkManager = (DummyServerNetworkManager)this.networkManager;
            var requestGameStartCommand = NetworkCommandData.From<RequestGameStart>();
            networkManager.FakeReceiveMessage(this.ConnectionId, requestGameStartCommand.ToString());
        }
    }
}
