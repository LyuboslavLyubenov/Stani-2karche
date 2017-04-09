using NetworkCommandData = Commands.NetworkCommandData;

namespace Tests.Network.VoteForAnswerForCurrentQuestionColletor
{

    using System.Collections.Generic;
    using System.Linq;

    using Interfaces;
    using Interfaces.Network;
    using Interfaces.Network.NetworkManager;

    using Tests.DummyObjects;

    using UnityEngine;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Utils.Unity;

    using Zenject.Source.Usage;

    public class WhenStartedCollectingSendMainPlayersLoadQuestionCommand : ExtendedMonoBehaviour
    {
        [Inject]
        private ICollectVoteResultForAnswerForCurrentQuestion voteCollector;

        [Inject]
        private IServerNetworkManager networkManager;

        [Inject]
        private ISimpleQuestion question;

        [Inject]
        private IEveryBodyVsTheTeacherServer server;

        private List<int> clientsReceivedQuestion = new List<int>();

        void Start()
        {
            var dummyServerNetworkManager = (DummyServerNetworkManager)this.networkManager;
            var questionJSON = JsonUtility.ToJson(this.question.Serialize());

            dummyServerNetworkManager.OnSentDataToClient += (sender, args) =>
                {
                    var command = NetworkCommandData.Parse(args.Message);
                    if (this.server.MainPlayersConnectionIds.Contains(args.ConnectionId) &&
                        command.Name == "LoadQuestion" &&
                        command.Options["QuestionJSON"] == questionJSON)
                    {
                        this.clientsReceivedQuestion.Add(args.ConnectionId);
                    }
                };
            
            this.voteCollector.StartCollecting();

            this.CoroutineUtils.WaitForSeconds(0.5f,
                () =>
                    {
                        if (this.server.MainPlayersConnectionIds.All(this.clientsReceivedQuestion.Contains))
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