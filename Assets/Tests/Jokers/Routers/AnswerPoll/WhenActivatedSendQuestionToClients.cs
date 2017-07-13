using NetworkCommandData = Commands.NetworkCommandData;

namespace Tests.Jokers.Routers.AnswerPoll
{

    using System.Collections.Generic;
    using System.Linq;

    using Interfaces;
    using Interfaces.Network.Jokers.Routers;
    using Interfaces.Network.NetworkManager;

    using Tests.DummyObjects;

    using UnityEngine;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Utils.Unity;

    using Zenject;

    public class WhenActivatedSendQuestionToClients : ExtendedMonoBehaviour
    {
        [Inject]
        private IServerNetworkManager networkManager;

        [Inject]
        private IAnswerPollRouter answerPollRouter;

        [Inject]
        private ISimpleQuestion simpleQuestion;

        void Start()
        {
            var clientsReceivedQuestionJSON = new List<int>();
            var clientsToVote = new[]
                                {
                                    1,
                                    2,
                                    3,
                                    4
                                };
            var questionJSON = JsonUtility.ToJson(this.simpleQuestion.Serialize());
            
            ((DummyServerNetworkManager)this.networkManager).OnSentDataToClient += (sender, args) =>
            {
                var networkData = NetworkCommandData.Parse(args.Message);
                if (
                    networkData.Name == "LoadQuestion" &&
                    networkData.Options.First(o => o.Key == "QuestionJSON").Value == questionJSON)
                {
                    Debug.Log(questionJSON);
                    clientsReceivedQuestionJSON.Add(args.ConnectionId);
                }
            };

            this.answerPollRouter.Activate(5, clientsToVote, this.simpleQuestion);
            
            this.CoroutineUtils.WaitForSeconds(5.5f, 
                () =>
                    {
                        this.answerPollRouter.Deactivate();
                        this.answerPollRouter.Dispose();

                        var areAllClientsReceivedQuestionJson = clientsToVote.All(c => clientsReceivedQuestionJSON.Contains(c));
                        if (areAllClientsReceivedQuestionJson)
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