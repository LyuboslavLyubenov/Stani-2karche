using NetworkCommandData = Commands.NetworkCommandData;

namespace Tests.Jokers.Routers.AnswerPoll
{

    using System.Collections.Generic;
    using System.Linq;

    using Interfaces;
    using Interfaces.Network.Jokers.Routers;
    using Interfaces.Network.NetworkManager;

    using Tests.DummyObjects;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Utils.Unity;

    using Zenject;

    public class AfterFinishedShowNotificationIsOver : ExtendedMonoBehaviour
    {

        [Inject]
        private IServerNetworkManager networkManager;

        [Inject]
        private IAnswerPollRouter answerPollRouter;

        [Inject]
        private ISimpleQuestion simpleQuestion;


        void Start()
        {
            var clients = new int[]
                          {
                              1,
                              2,
                              3,
                              4
                          };
            var clientsReceivedShowNotificationCommand = new List<int>();
            
            ((DummyServerNetworkManager)this.networkManager).OnSentDataToClient += (sender, args) =>
                {
                    var command = NetworkCommandData.Parse(args.Message);
                    if (command.Name == "ShowNotification")
                    {
                        clientsReceivedShowNotificationCommand.Add(args.ConnectionId);
                    }
                };
            
            this.answerPollRouter.Activate(5, clients, this.simpleQuestion);

            var selectedAnswer = new NetworkCommandData("AnswerSelected");
            selectedAnswer.AddOption("Answer", this.simpleQuestion.Answers[0]);

            for (int i = 0; i < clients.Length; i++)
            {
                var connectionId = clients[i];
                ((DummyServerNetworkManager)this.networkManager).FakeReceiveMessage(connectionId, selectedAnswer.ToString());
            }

            this.CoroutineUtils.WaitForSeconds(3,
                () =>
                    {
                        if (clientsReceivedShowNotificationCommand.All(c => clients.Contains(c)))
                        {
                            IntegrationTest.Pass();
                        }
                        else
                        {
                            IntegrationTest.Fail();
                        }
                    });
        }
    }
}
