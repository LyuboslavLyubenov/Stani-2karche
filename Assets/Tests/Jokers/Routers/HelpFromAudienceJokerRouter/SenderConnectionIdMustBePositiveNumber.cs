namespace Assets.Tests.Jokers.Routers.HelpFromAudienceJokerRouter
{

    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Interfaces.GameData;
    using Assets.Scripts.Interfaces.Network.Jokers.Routers;
    using Assets.Scripts.Interfaces.Network.NetworkManager;
    using Assets.Tests.DummyObjects;
    using Assets.Tests.Extensions;
    using Assets.Zenject.Source.Usage;

    using UnityEngine;

    public class SenderConnectionIdMustBePositiveNumber : MonoBehaviour
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

            this.helpFromAudienceJokerRouter.Activate(-1, 60);
        }
    }
}
