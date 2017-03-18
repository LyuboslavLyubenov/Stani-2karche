using NetworkCommandData = Commands.NetworkCommandData;

namespace Tests.Jokers.Routers.HelpFromAudienceJokerRouter
{

    using Interfaces;
    using Interfaces.GameData;
    using Interfaces.Network.Jokers.Routers;
    using Interfaces.Network.NetworkManager;

    using Tests.DummyObjects;
    using Tests.Extensions;

    using UnityEngine;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Zenject.Source.Usage;

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
