using NetworkCommandData = Commands.NetworkCommandData;

namespace Tests.UI.EverybodyVsTheTeacher
{

    using Interfaces.Network.NetworkManager;

    using Tests.DummyObjects;

    using Utils.Unity;

    using Zenject;

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
