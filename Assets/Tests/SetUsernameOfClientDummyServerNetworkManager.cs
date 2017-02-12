namespace Assets.Tests.UI.EverybodyVsTheTeacher.AudiencePlayersContainerUI
{

    using System.Collections.Generic;

    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Utils.Unity;

    using Zenject;

    public class SetUsernameOfClientDummyServerNetworkManager : ExtendedMonoBehaviour
    {
        public string Username;

        public int ConnectionId;

        public float SetAfterTimeInSeconds = 1f;

        [Inject]
        private IServerNetworkManager networkManager;

        void Start()
        {
            CoroutineUtils.WaitForSeconds(this.SetAfterTimeInSeconds, this.SetUsername);
        }

        private void SetUsername()
        {
            ((DummyServerNetworkManager)this.networkManager).FakeSetUsernameToPlayer(this.ConnectionId, this.Username);
        }
    }

}