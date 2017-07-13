using DummyEveryBodyVsTheTeacherServer = Tests.DummyObjects.DummyEveryBodyVsTheTeacherServer;
using DummyServerNetworkManager = Tests.DummyObjects.DummyServerNetworkManager;
using ExtendedMonoBehaviour = Utils.Unity.ExtendedMonoBehaviour;
using NetworkCommandData = Commands.NetworkCommandData;

namespace Tests.Network.VoteForAnswerForCurrentQuestionColletor
{

    using Assets.Scripts.Extensions;
    using Assets.Tests.Extensions;

    using Interfaces;
    using Interfaces.Network;
    using Interfaces.Network.NetworkManager;

    using UnityEngine;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Zenject;

    public class WhenMainPlayerReconnectedSendQuestionWithRemainingTime : ExtendedMonoBehaviour
    {
        [Inject]
        private IServerNetworkManager networkManager;

        [Inject]
        private IEveryBodyVsTheTeacherServer server;

        [Inject]
        private ICollectVoteResultForAnswerForCurrentQuestion voteResultCollector;

        [Inject]
        private ISimpleQuestion question;

        [Inject]
        private int timeToAnswer;

        void Start()
        {
            var dummyServer = (DummyEveryBodyVsTheTeacherServer)this.server;
            dummyServer.ConnectedMainPlayersConnectionIds = new int[] { 1, 2, 3, 4, 5 };
            dummyServer.PresenterId = 6;

            this.voteResultCollector.StartCollecting();

            this.CoroutineUtils.WaitForSeconds(1,
                () =>
                    {
                        var questionDto = this.question.Serialize();
                        var questionJSON = JsonUtility.ToJson(questionDto);
                        var dummyNetworkManager = (DummyServerNetworkManager)this.networkManager;
                        dummyNetworkManager.OnSentDataToClient += (sender, args) =>
                            {
                                var command = NetworkCommandData.Parse(args.Message);

                                if (
                                    command.Name == "LoadQuestion" &&
                                    command.Options["QuestionJSON"] == questionJSON &&
                                    command.Options["TimeToAnswer"].ConvertTo<int>() < this.timeToAnswer)
                                {
                                    IntegrationTest.Pass();
                                }
                            };

                        dummyServer.ConnectedMainPlayersConnectionIds = new int[] { 1, 3, 4, 5 };
                        dummyNetworkManager.FakeDisconnectPlayer(2);

                        this.CoroutineUtils.WaitForFrames(1,
                            () =>
                                {
                                    dummyServer.ConnectedMainPlayersConnectionIds = new int[] { 1, 2, 3, 4, 5 };
                                    dummyNetworkManager.SimulateMainPlayerConnected(1, "Ivan");
                                });
                    });
        }
    }
}