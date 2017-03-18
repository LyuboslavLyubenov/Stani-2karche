using NetworkCommandData = Commands.NetworkCommandData;

namespace Tests.Jokers.Routers.AnswerPoll
{

    using System.Linq;

    using Interfaces;
    using Interfaces.Network.Jokers.Routers;
    using Interfaces.Network.NetworkManager;

    using Tests.DummyObjects;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Utils.Unity;

    using Zenject.Source.Usage;

    public class AllClientsVotedCorrectly : ExtendedMonoBehaviour
    {
        [Inject]
        private IServerNetworkManager networkManager;

        [Inject]
        private IAnswerPollRouter answerPollRouter;

        [Inject]
        private ISimpleQuestion simpleQuestion;

        void Start()
        {
            var clientsToVote = new[]
                                {
                                    1,
                                    2,
                                    3,
                                    4
                                };

            this.answerPollRouter.Activate(5, clientsToVote, this.simpleQuestion);
            
            this.answerPollRouter.OnVoteFinished += (sender, args) =>
            {
                var highestVotedAnswerVotedCount = args.AnswersVotes.OrderByDescending(av => av.Value)
                    .First();

                this.answerPollRouter.Deactivate();
                this.answerPollRouter.Dispose();

                if (
                    highestVotedAnswerVotedCount.Key == this.simpleQuestion.Answers[0] &&
                    highestVotedAnswerVotedCount.Value == clientsToVote.Length)
                {
                    IntegrationTest.Pass();
                }
                else
                {
                    IntegrationTest.Fail();
                }   
            };

            for (int i = 0; i < clientsToVote.Length; i++)
            {
                var clientId = clientsToVote[i];
                var selectedAnswer = new NetworkCommandData("AnswerSelected");
                selectedAnswer.AddOption("Answer", this.simpleQuestion.Answers[0]);
                ((DummyServerNetworkManager)this.networkManager).FakeReceiveMessage(clientId, selectedAnswer.ToString());
            }
        }
    }

}