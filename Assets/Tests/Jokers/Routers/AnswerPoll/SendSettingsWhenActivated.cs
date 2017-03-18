using NetworkCommandData = Commands.NetworkCommandData;

namespace Tests.Jokers.Routers.AnswerPoll
{

    using System.Collections.Generic;
    using System.Linq;

    using Interfaces;
    using Interfaces.Network.Jokers.Routers;
    using Interfaces.Network.NetworkManager;

    using Tests.DummyObjects;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Utils.Unity;

    using Zenject.Source.Usage;

    public class SendSettingsWhenActivated : ExtendedMonoBehaviour
    {
        [Inject]
        private IServerNetworkManager networkManager;

        [Inject]
        private IAnswerPollRouter answerPollRouter;

        [Inject]
        private ISimpleQuestion simpleQuestion;

        void Start()
        {
            var votedClients = new List<int>();
            var clientsToVote = new[]
                                {
                                    1,
                                    2,
                                    3,
                                    4,
                                    5,
                                    6
                                };

            ((DummyServerNetworkManager)this.networkManager).OnSentDataToClient += (sender, args) =>
            {
                var networkData = NetworkCommandData.Parse(args.Message);
                if (
                    networkData.Name == "AnswerPollSettings" &&
                    networkData.Options.First(o => o.Key == "TimeToAnswerInSeconds").Value == "5")
                {
                    votedClients.Add(args.ConnectionId);
                }
            };

            this.answerPollRouter.Activate(5, clientsToVote, this.simpleQuestion);
            
            this.CoroutineUtils.WaitForSeconds(5.1f,
                () =>
                    {
                        this.answerPollRouter.Deactivate();
                        this.answerPollRouter.Dispose();

                        var areAllClientsReceivedSettings = clientsToVote.All(c => votedClients.Contains(c));
                        if (areAllClientsReceivedSettings)
                        {
                            IntegrationTest.Pass();
                        }
                        else
                        {
                            IntegrationTest.Fail();
                        }
                    });
        }
    }

}