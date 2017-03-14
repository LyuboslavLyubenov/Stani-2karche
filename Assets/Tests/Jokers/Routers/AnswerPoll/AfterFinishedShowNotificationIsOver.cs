namespace Assets.Tests.Jokers.Routers.AnswerPoll
{

    using System.Collections.Generic;
    using System.Linq;

    using Assets.Scripts.Commands;
    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Interfaces.Network.Jokers;
    using Assets.Scripts.Interfaces.Network.NetworkManager;
    using Assets.Scripts.Utils.Unity;
    using Assets.Tests.DummyObjects;
    using Assets.UnityTestTools.IntegrationTestsFramework.TestRunner;
    using Assets.Zenject.Source.Usage;

    using UnityEngine;

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
