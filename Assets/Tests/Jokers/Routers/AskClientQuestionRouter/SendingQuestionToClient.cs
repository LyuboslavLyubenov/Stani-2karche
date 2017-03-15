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

    public class SendingQuestionToClient : MonoBehaviour
    {
        [Inject]
        private ISimpleQuestion question;

        [Inject]
        private IServerNetworkManager networkManager;

        [Inject]
        private IAskClientQuestionRouter askClientQuestionRouter;

        void Start()
        {
            var dummyServerNetworkManager = (DummyServerNetworkManager)this.networkManager;
            var clientConnectionId = 1;
            var questionJSON = JsonUtility.ToJson(this.question.Serialize());

            dummyServerNetworkManager.SimulateClientConnected(clientConnectionId, "Ivan");
            dummyServerNetworkManager.OnSentDataToClient += (sender, args) =>
                {
                    var command = NetworkCommandData.Parse(args.Message);
                    if (command.Name == "LoadQuestion" && 
                        args.ConnectionId == clientConnectionId &&
                        command.Options["QuestionJSON"] == questionJSON
                        )
                    {
                        IntegrationTest.Pass();
                    }
                };

            this.askClientQuestionRouter.Activate(clientConnectionId, 5, this.question);
        }
    }
}
