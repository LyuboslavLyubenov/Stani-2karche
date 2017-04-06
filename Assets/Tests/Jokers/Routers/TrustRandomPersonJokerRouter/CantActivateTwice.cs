using DummyServerNetworkManager = Tests.DummyObjects.DummyServerNetworkManager;

namespace Tests.Jokers.Routers.TrustRandomPersonJokerRouter
{

    using Assets.Scripts.Interfaces.Network.Jokers.Routers;

    using Extensions;

    using Interfaces.Network;
    using Interfaces.Network.NetworkManager;

    using Tests.DummyObjects;

    using UnityEngine;

    using Zenject.Source.Usage;

    public class CantActivateTwice : MonoBehaviour
    {
        [Inject]
        private IServerNetworkManager networkManager;

        [Inject]
        private IEveryBodyVsTheTeacherServer server;

        [Inject]
        private ITrustRandomPersonJokerRouter jokerRouter;

        void Start()
        {
            this.jokerRouter.Activate();
            this.jokerRouter.Activate();
        }
    }
}
