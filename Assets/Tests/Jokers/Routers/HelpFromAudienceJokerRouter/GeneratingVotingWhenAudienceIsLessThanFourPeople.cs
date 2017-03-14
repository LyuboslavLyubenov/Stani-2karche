namespace Assets.Tests.Jokers.Routers.HelpFromAudienceJokerRouter
{
    using Assets.Scripts.Commands;
    using Assets.Scripts.DTOs;
    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Interfaces.GameData;
    using Assets.Scripts.Interfaces.Network.Jokers.Routers;
    using Assets.Scripts.Interfaces.Network.NetworkManager;
    using Assets.Tests.DummyObjects;
    using Assets.Tests.Extensions;
    using Assets.UnityTestTools.IntegrationTestsFramework.TestRunner;
    using Assets.Zenject.Source.Usage;

    using UnityEngine;

    public class GeneratingVotingWhenAudienceIsLessThanFourPeople : MonoBehaviour
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
            var dummyNetworkManager = ((DummyServerNetworkManager)this.networkManager);

            for (int i = 1; i <= 2; i++)
            {
                dummyNetworkManager.SimulateClientConnected(i, "Ivan" + i);
            }

            dummyNetworkManager.OnSentDataToClient += (sender, args) =>
                {
                    var command = NetworkCommandData.Parse(args.Message);

                    if (command.Name == "AudienceAnswerPollResult")
                    {
                        IntegrationTest.Pass();
                    }
                };

            this.helpFromAudienceJokerRouter.Activate(1, 5);
        }
    }
}
