namespace Assets.Tests.UI.EverybodyVsTheTeacher.AudiencePlayersContainerUI
{
    using System.Collections;

    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Utils.Unity;

    using UnityEngine;

    using Zenject;

    public class AddClientsToDummyServerNetworkManager : ExtendedMonoBehaviour
    {
        public int Count = 1;

        [Inject]
        private IServerNetworkManager networkManager;

        void Start()
        {
            this.StartCoroutine(this.SimulateConnectedClients());
        }

        void OnDisable()
        {
            var serverNetworkManager = (DummyServerNetworkManager)this.networkManager;

            for (int i = 0; i < this.Count; i++)
            {
                serverNetworkManager.FakeDisconnectPlayer(i + 1);
            }
        }

        IEnumerator SimulateConnectedClients()
        {
            var serverNetworkManager = (DummyServerNetworkManager)this.networkManager;

            for (int i = 0; i < this.Count; i++)
            {
                yield return new WaitForSeconds(1f);
                serverNetworkManager.FakeConnectPlayer((i + 1));
            }
        }
    }

}
