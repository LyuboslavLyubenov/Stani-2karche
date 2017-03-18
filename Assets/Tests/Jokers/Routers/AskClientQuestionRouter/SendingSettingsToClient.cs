using NetworkCommandData = Commands.NetworkCommandData;

namespace Tests.Jokers.Routers.AskClientQuestionRouter
{

    using System.Linq;

    using Interfaces;
    using Interfaces.Network.Jokers.Routers;
    using Interfaces.Network.NetworkManager;

    using Tests.DummyObjects;
    using Tests.Extensions;

    using UnityEngine;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Zenject.Source.Usage;

    public class SendingSettingsToClient : MonoBehaviour
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
            
            var timeToAnswerInSeconds = 5;

            dummyServerNetworkManager.OnSentDataToClient += (sender, args) =>
                {
                    var command = NetworkCommandData.Parse(args.Message);
                    if (command.Name == "AskPlayerQuestionSettings" &&
                        args.ConnectionId == clientConnectionId &&
                        command.Options.First(o => o.Key == "TimeToAnswerInSeconds").Value == timeToAnswerInSeconds.ToString())
                    {
                        IntegrationTest.Pass();
                    }
                };

            this.askClientQuestionRouter.Activate(clientConnectionId, timeToAnswerInSeconds, this.question);
        }
    }
}