namespace Tests.Jokers.Routers.HelpFromAudienceJokerRouter
{

    using Assets.Tests.Extensions;

    using Interfaces;
    using Interfaces.GameData;
    using Interfaces.Network.Jokers.Routers;
    using Interfaces.Network.NetworkManager;

    using Tests.DummyObjects;

    using UnityEngine;

    using Zenject.Source.Usage;

    public class CanActivateOnlyOneTime : MonoBehaviour
    {
        [Inject]
        private ISimpleQuestion question;

        [Inject]
        private IGameDataIterator gameDataIterator;

        [Inject]
        private IServerNetworkManager networkManager;

        [Inject]
        private IHelpFromAudienceJokerRouter helpFromAudienceJokerRouter;

        void Start()
        {
            for (int i = 1; i < 10; i++)
            {
                ((DummyServerNetworkManager)this.networkManager).SimulateClientConnected(i, "Ivan" + i);
            }

            for (int i = 0; i < 2; i++)
            {
                this.helpFromAudienceJokerRouter.Activate(1, 60);
            }
        }
    }
}
