namespace Assets.Tests.Jokers.Routers.AskClientQuestionRouter
{
    using Assets.Scripts.Commands;
    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Interfaces.Network.Jokers.Routers;
    using Assets.Scripts.Interfaces.Network.NetworkManager;
    using Assets.Tests.DummyObjects;
    using Assets.Tests.Extensions;
    using Assets.UnityTestTools.IntegrationTestsFramework.TestRunner;
    using Assets.Zenject.Source.Usage;

    using UnityEngine;

    public class ReceiveAnswerOnlyIfSelectedClientVoted : MonoBehaviour
    {
        [Inject]
        private ISimpleQuestion question;

        [Inject]
        private IServerNetworkManager networkManager;

        [Inject]
        private IAskClientQuestionRouter askClientQuestionRouter;

        void Start()
        {
            var dummyServerNetworkManager = ((DummyServerNetworkManager)this.networkManager);
            var clientConnectionId = 1;
            dummyServerNetworkManager.SimulateClientConnected(clientConnectionId, "Ivan");

            var selectedAnswer = this.question.Answers[0];
            this.askClientQuestionRouter.OnReceivedAnswer += (sender, args) =>
                {
                    if (args.Answer == selectedAnswer)
                    {
                        IntegrationTest.Pass();
                    }
                };
            this.askClientQuestionRouter.Activate(clientConnectionId, 5, this.question);

            var selectedAnswerCommand = new NetworkCommandData("AnswerSelected");
            selectedAnswerCommand.AddOption("Answer", selectedAnswer);
            dummyServerNetworkManager.FakeReceiveMessage(clientConnectionId, selectedAnswerCommand.ToString());
        }
    }
}