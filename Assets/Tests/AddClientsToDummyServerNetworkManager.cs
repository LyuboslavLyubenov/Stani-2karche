namespace Tests
{

    using System.Collections;

    using Interfaces.Network.NetworkManager;

    using Tests.DummyObjects;

    using UnityEngine;

    using Utils.Unity;

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
                serverNetworkManager.FakeConnectPlayer((i + 1));
                yield return new WaitForSeconds(1f);
            }
        }
    }

}
