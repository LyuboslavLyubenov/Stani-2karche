using NetworkCommandData = Commands.NetworkCommandData;

namespace Tests.Jokers.Routers.TrustRandomPersonJokerRouter
{
    using Assets.Scripts.Interfaces.Network.Jokers.Routers;

    using Interfaces;
    using Interfaces.Network;
    using Interfaces.Network.NetworkManager;

    using Tests.DummyObjects;
    using Tests.Extensions;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Utils.Unity;

    using Zenject.Source.Usage;

    public class ReceivedAnswerFromPlayer : ExtendedMonoBehaviour
    {
        [Inject]
        private IServerNetworkManager serverNetworkManager;

        [Inject]
        private ISimpleQuestion question;

        [Inject]
        private IEveryBodyVsTheTeacherServer server;

        [Inject]
        private ITrustRandomPersonJokerRouter trustRandomPersonJokerRouter;

        void Start()
        {
            var answer = this.question.Answers[0];

            this.trustRandomPersonJokerRouter.OnReceivedAnswer += (sender, args) =>
                {
                    if (args.Answer == answer)
                    {
                        IntegrationTest.Pass();
                    }
                };

            this.trustRandomPersonJokerRouter.Activate();
            
            this.CoroutineUtils.WaitForSeconds(0.1f,
                () =>
                    {
                        var dummyServerNetworkManager = (DummyServerNetworkManager)this.serverNetworkManager;
                        var answerSelectedCommand = new NetworkCommandData("AnswerSelected");
                        answerSelectedCommand.AddOption("Answer", answer);
                        dummyServerNetworkManager.FakeReceiveMessage(1, answerSelectedCommand.ToString());
                    });
        }        
    }
}