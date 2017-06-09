using DummyEveryBodyVsTheTeacherServer = Tests.DummyObjects.DummyEveryBodyVsTheTeacherServer;
using DummyServerNetworkManager = Tests.DummyObjects.DummyServerNetworkManager;
using ExtendedMonoBehaviour = Utils.Unity.ExtendedMonoBehaviour;

namespace Assets.Tests.Network.VoteForAnswerForCurrentQuestionColletor
{

    using Assets.Scripts.Commands.EveryBodyVsTheTeacher;
    using Assets.Scripts.Extensions;

    using Commands;

    using Extensions;

    using Interfaces;
    using Interfaces.Network;
    using Interfaces.Network.NetworkManager;

    using UnityEngine;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Zenject.Source.Usage;

    public class WhenPresenterReconnectedSendQuestionWithRemainingTime : ExtendedMonoBehaviour
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
            dummyServer.MainPlayersConnectionIds = new int[] { 1, 2, 3, 4, 5 };
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

                        dummyServer.MainPlayersConnectionIds = new int[] { 1, 2, 3, 4, 5 };
                        dummyNetworkManager.FakeDisconnectPlayer(6);

                        this.CoroutineUtils.WaitForFrames(1,
                            () =>
                                {
                                    dummyNetworkManager.FakeConnectPlayer(6);
                                    var presenterConnectingCommand =
                                        NetworkCommandData.From<PresenterConnectingCommand>();
                                    dummyNetworkManager.FakeReceiveMessage(6, presenterConnectingCommand.ToString());
                                });
                    });
        }
    }

}