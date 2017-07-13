using MainPlayerConnectingCommand = Commands.Server.MainPlayerConnectingCommand;
using NetworkCommandData = Commands.NetworkCommandData;

namespace Tests.Network
{

    using Interfaces.Network.NetworkManager;

    using Tests.DummyObjects;

    using Utils.Unity;

    using Zenject;

    public class MakeClientMainPlayer : ExtendedMonoBehaviour
    {
        public float SetAfterTimeInSeconds = 1f;
        public int ConnectionId = 1;

        [Inject]
        private IServerNetworkManager serverNetworkManager;
    
        void Start ()
        {
            this.CoroutineUtils.WaitForSeconds(this.SetAfterTimeInSeconds, this.MakeAsMainPlayer);		
        }

        void MakeAsMainPlayer()
        {
            var commandData = NetworkCommandData.From<MainPlayerConnectingCommand>();
            ((DummyServerNetworkManager)this.serverNetworkManager).FakeReceiveMessage(this.ConnectionId, commandData.ToString());
        }
	
    }

}
