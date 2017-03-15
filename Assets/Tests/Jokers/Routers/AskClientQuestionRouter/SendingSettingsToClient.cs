namespace Assets.Tests.Jokers.Routers.AskClientQuestionRouter
{

    using System.Linq;

    using Assets.Scripts.Commands;
    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Interfaces.Network.Jokers.Routers;
    using Assets.Scripts.Interfaces.Network.NetworkManager;
    using Assets.Tests.DummyObjects;
    using Assets.Tests.Extensions;
    using Assets.UnityTestTools.IntegrationTestsFramework.TestRunner;
    using Assets.Zenject.Source.Usage;

    using UnityEngine;

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