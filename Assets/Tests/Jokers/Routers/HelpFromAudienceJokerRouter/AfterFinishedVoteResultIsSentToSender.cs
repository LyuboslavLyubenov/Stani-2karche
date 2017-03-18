namespace Assets.Tests.Jokers.Routers.HelpFromAudienceJokerRouter
{
    using System.Collections.Generic;
    using System.Linq;

    using Assets.Scripts.Commands;
    using Assets.Scripts.Extensions;
    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Interfaces.GameData;
    using Assets.Scripts.Interfaces.Network.Jokers;
    using Assets.Scripts.Interfaces.Network.Jokers.Routers;
    using Assets.Scripts.Interfaces.Network.NetworkManager;
    using Assets.Scripts.Utils.Unity;
    using Assets.Tests.DummyObjects;
    using Assets.Tests.Extensions;
    using Assets.UnityTestTools.IntegrationTestsFramework.TestRunner;
    using Assets.Zenject.Source.Usage;

    using UnityEngine;

    public class AfterFinishedVoteResultIsSentToSender : ExtendedMonoBehaviour
    {
        [Inject]
        private ISimpleQuestion question;

        [Inject]
        private IGameDataIterator gameDataIterator;

        [Inject]
        private IServerNetworkManager networkManager;

        [Inject]
        private IHelpFromAudienceJokerRouter helpFromAudienceJokerRouter;

        [Inject]
        private IAnswerPollRouter answerPollRouter;

        void Start()
        {
            var senderId = 1;
            var dummyNetworkManager = ((DummyServerNetworkManager)this.networkManager);

            for (int i = 1; i <= 6; i++)
            {
                dummyNetworkManager.SimulateClientConnected(i, "Ivan" + i);
            }
            
            var votes = new Dictionary<string, int>()
                        {
                            { this.question.Answers[0], 5 },
                            { this.question.Answers[1], 2 },
                            { this.question.Answers[2], 1 },
                            { this.question.Answers[3], 0 }
                        };

            dummyNetworkManager.OnSentDataToClient += (sender, args) =>
            {
                var command = NetworkCommandData.Parse(args.Message);
                if (command.Name == "AudienceAnswerPollResult")
                {
                    var areVotesInSameState = command.Options.All(o => votes[o.Key] == o.Value.ConvertTo<int>());
                    
                    if (args.ConnectionId == senderId && areVotesInSameState)
                    {
                        this.helpFromAudienceJokerRouter.Dispose();
                        IntegrationTest.Pass();
                    }
                }
            };

            this.helpFromAudienceJokerRouter.Activate(senderId, 5);

            this.CoroutineUtils.WaitForFrames(10,
                () =>
                    {
                        ((DummyAnswerPollRouter)this.answerPollRouter).SimulateVoteFinished(votes);
                    });
        }
    }
}