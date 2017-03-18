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

    public class OneCantVoteMultipleTimes : ExtendedMonoBehaviour
    {
        [Inject]
        private IServerNetworkManager networkManager;

        [Inject]
        private IAnswerPollRouter answerPollRouter;

        [Inject]
        private ISimpleQuestion simpleQuestion;

        void Start()
        {
            this.answerPollRouter.Activate(
                5,
                new[]
                {
                    1,
                    2,
                    3
                },
                this.simpleQuestion);
            this.answerPollRouter.OnVoteFinished += (sender, args) =>
            {
                this.answerPollRouter.Deactivate();
                this.answerPollRouter.Dispose();

                var highestVotedAnswer = args.AnswersVotes.OrderByDescending(av => av.Value)
                    .First();

                if (highestVotedAnswer.Value == 1)
                {
                    IntegrationTest.Pass();
                }
                else
                {
                    IntegrationTest.Fail();
                }
            };

            var selectedAnswer = new NetworkCommandData("AnswerSelected");
            selectedAnswer.AddOption("Answer", this.simpleQuestion.Answers[0]);
            
            for (int i = 0; i < 3; i++)
            {
                ((DummyServerNetworkManager)this.networkManager).FakeReceiveMessage(1, selectedAnswer.ToString());
            }
        }
    }
}