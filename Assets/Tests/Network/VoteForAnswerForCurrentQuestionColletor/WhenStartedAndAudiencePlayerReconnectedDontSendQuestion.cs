using DummyEveryBodyVsTheTeacherServer = Tests.DummyObjects.DummyEveryBodyVsTheTeacherServer;
using DummyServerNetworkManager = Tests.DummyObjects.DummyServerNetworkManager;
using ExtendedMonoBehaviour = Utils.Unity.ExtendedMonoBehaviour;

namespace Assets.Tests.Network.VoteForAnswerForCurrentQuestionColletor
{

    using Commands;

    using Interfaces.Network;
    using Interfaces.Network.NetworkManager;

    using UnityEngine;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Zenject.Source.Usage;

    public class WhenStartedAndAudiencePlayerReconnectedDontSendQuestion : ExtendedMonoBehaviour
    {
        [Inject]
        private IServerNetworkManager networkManager;

        [Inject]
        private IEveryBodyVsTheTeacherServer server;

        [Inject]
        private ICollectVoteResultForAnswerForCurrentQuestion voteResultCollector;

        void Start()
        {
            var dummyServer = (DummyEveryBodyVsTheTeacherServer)this.server;
            dummyServer.ConnectedMainPlayersConnectionIds = new int[] {1, 2, 3};
            dummyServer.PresenterId = 4;

            this.voteResultCollector.StartCollecting();

            this.CoroutineUtils.WaitForFrames(1,
                () =>
                    {
                        var dummyNetworkManager = (DummyServerNetworkManager)this.networkManager;
                        dummyNetworkManager.OnSentDataToClient += (sender, args) =>
                            {
                                var command = NetworkCommandData.Parse(args.Message);
                                if (command.Name == "LoadQuestion")
                                {
                                    IntegrationTest.Fail();
                                }
                            };

                        dummyNetworkManager.FakeDisconnectPlayer(10);

                        this.CoroutineUtils.WaitForFrames(1,
                            () =>
                                {
                                    dummyNetworkManager.FakeConnectPlayer(10);
                                    this.CoroutineUtils.WaitForFrames(1, IntegrationTest.Pass);
                                });
                    });
        }
    }
}