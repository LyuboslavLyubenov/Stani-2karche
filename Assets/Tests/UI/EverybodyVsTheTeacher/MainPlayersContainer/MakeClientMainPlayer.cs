namespace Assets.Tests.UI.EverybodyVsTheTeacher.MainPlayersContainer
{

    using Assets.Scripts.Commands;
    using Assets.Scripts.Commands.Server;
    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Interfaces.Network.NetworkManager;
    using Assets.Scripts.Utils.Unity;
    using Assets.Zenject.Source.Usage;

    public class MakeClientMainPlayer : ExtendedMonoBehaviour
    {
        public float SetAfterTimeInSeconds = 1f;
        public int ConnectionId = 1;

        [Inject]
        private IServerNetworkManager serverNetworkManager;
    
        void Start ()
        {
            CoroutineUtils.WaitForSeconds(this.SetAfterTimeInSeconds, this.MakeAsMainPlayer);		
        }

        void MakeAsMainPlayer()
        {
            var commandData = NetworkCommandData.From<MainPlayerConnectingCommand>();
            ((DummyServerNetworkManager)this.serverNetworkManager).FakeReceiveMessage(this.ConnectionId, commandData.ToString());
        }
	
    }

}
