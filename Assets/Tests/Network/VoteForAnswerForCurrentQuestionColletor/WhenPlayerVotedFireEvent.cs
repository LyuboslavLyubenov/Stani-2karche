using DummyServerNetworkManager = Tests.DummyObjects.DummyServerNetworkManager;
using ExtendedMonoBehaviour = Utils.Unity.ExtendedMonoBehaviour;
using NetworkCommandDataClass = Commands.NetworkCommandData;

namespace Tests.Network.VoteForAnswerForCurrentQuestionColletor
{

    using Assets.Tests.Extensions;

    using Interfaces;
    using Interfaces.Network;
    using Interfaces.Network.NetworkManager;
    
    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Zenject;

    public class WhenPlayerVotedFireEvent : ExtendedMonoBehaviour
    {
        [Inject]
        private IServerNetworkManager networkManager;

        [Inject]
        private ICollectVoteResultForAnswerForCurrentQuestion collectVoteResult;

        [Inject]
        private ISimpleQuestion question;

        void Start()
        {
            var dummyServerNetworkManager = (DummyServerNetworkManager)this.networkManager;
            dummyServerNetworkManager.SimulateClientConnected(1, "Player 1");
            dummyServerNetworkManager.SimulateClientConnected(2, "Player 2");

            this.collectVoteResult.OnPlayerVoted += (sender, args) =>
                {
                    if (args.Answer == this.question.CorrectAnswer)
                    {
                        IntegrationTest.Pass();
                    }
                    else
                    {
                        IntegrationTest.Fail();
                    }
                };
            
            this.CoroutineUtils.WaitForFrames(1,
                () =>
                    {
                        this.collectVoteResult.StartCollecting();

                        this.CoroutineUtils.WaitForFrames(1,
                            () =>
                                {
                                    var answerSelectedCommand = new NetworkCommandDataClass("AnswerSelected");
                                    answerSelectedCommand.AddOption("Answer", this.question.CorrectAnswer);
                                    dummyServerNetworkManager.FakeReceiveMessage(1, answerSelectedCommand.ToString());
                                });
                    });
        }
    }
}