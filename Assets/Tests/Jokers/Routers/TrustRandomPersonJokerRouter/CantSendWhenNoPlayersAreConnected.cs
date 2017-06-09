namespace Tests.Jokers.Routers.TrustRandomPersonJokerRouter
{

    using Interfaces.Network.Jokers.Routers;
    using Interfaces.Network.NetworkManager;

    using Tests.DummyObjects;

    using UnityEngine;

    using Utils.Unity;

    using Zenject.Source.Usage;

    public class CantSendWhenNoPlayersAreConnected : ExtendedMonoBehaviour
    {


        [Inject]
        private IServerNetworkManager networkManager;

        [Inject]
        private ITrustRandomPersonJokerRouter trustRandomPersonJokerRouter;

        void Start()
        {
            var dummyNetworkManager = (DummyServerNetworkManager)this.networkManager;

            dummyNetworkManager.FakeDisconnectPlayer(1);
            dummyNetworkManager.FakeDisconnectPlayer(2);
            
            this.trustRandomPersonJokerRouter.Activate();
        }
    }
}