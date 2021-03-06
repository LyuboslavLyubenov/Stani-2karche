using NetworkCommandData = Commands.NetworkCommandData;

namespace Tests.Jokers.Routers.TrustRandomPersonJokerRouter
{

    using Interfaces;
    using Interfaces.Network;
    using Interfaces.Network.Jokers.Routers;
    using Interfaces.Network.NetworkManager;

    using Tests.DummyObjects;

    using UnityEngine;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Zenject;

    public class SendingRandomClientQuestion : MonoBehaviour
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
            var dummyServerNetworkManager = (DummyServerNetworkManager)this.serverNetworkManager;

            dummyServerNetworkManager.OnSentDataToClient += (sender, args) =>
                {
                    var command = NetworkCommandData.Parse(args.Message);
                    if (
                        command.Name == "LoadQuestion" && 
                        args.ConnectionId != this.server.PresenterId &&
                        command.Options.ContainsKey("QuestionJSON") &&
                        command.Options["QuestionJSON"] == JsonUtility.ToJson(this.question.Serialize()))
                    {
                        IntegrationTest.Pass();
                    }
                };

            this.trustRandomPersonJokerRouter.Activate();
        }
    }
}