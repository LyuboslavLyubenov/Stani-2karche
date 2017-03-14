namespace Assets.Tests.Jokers.Routers.AnswerPoll
{

    using System;
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