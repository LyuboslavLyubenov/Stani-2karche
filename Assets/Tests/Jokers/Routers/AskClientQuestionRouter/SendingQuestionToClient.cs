using NetworkCommandData = Commands.NetworkCommandData;

namespace Tests.Jokers.Routers.AskClientQuestionRouter
{

    using Interfaces;
    using Interfaces.Network.Jokers.Routers;
    using Interfaces.Network.NetworkManager;

    using Tests.DummyObjects;
    using Tests.Extensions;

    using UnityEngine;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Zenject.Source.Usage;

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
