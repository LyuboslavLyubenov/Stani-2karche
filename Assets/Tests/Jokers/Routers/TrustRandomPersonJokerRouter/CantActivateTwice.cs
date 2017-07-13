namespace Tests.Jokers.Routers.TrustRandomPersonJokerRouter
{

    using Interfaces.Network;
    using Interfaces.Network.Jokers.Routers;
    using Interfaces.Network.NetworkManager;
    
    using UnityEngine;

    using Zenject;

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
