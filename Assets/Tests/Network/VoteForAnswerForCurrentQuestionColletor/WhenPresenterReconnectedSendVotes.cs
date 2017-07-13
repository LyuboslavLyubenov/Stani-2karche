using DummyEveryBodyVsTheTeacherServer = Tests.DummyObjects.DummyEveryBodyVsTheTeacherServer;
using DummyServerNetworkManager = Tests.DummyObjects.DummyServerNetworkManager;
using ExtendedMonoBehaviour = Utils.Unity.ExtendedMonoBehaviour;

namespace Assets.Tests.Network.VoteForAnswerForCurrentQuestionColletor
{
    using System.Linq;

    using Assets.Scripts.Commands.EveryBodyVsTheTeacher;
    using Assets.Tests.Extensions;

    using Commands;

    using Interfaces;
    using Interfaces.Network;
    using Interfaces.Network.NetworkManager;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Zenject;

    public class WhenPresenterReconnectedSendVotes : ExtendedMonoBehaviour
    {
        [Inject]
        private IServerNetworkManager networkManager;

        [Inject]
        private IEveryBodyVsTheTeacherServer server;

        [Inject]
        private ISimpleQuestion question;

        [Inject]
        private ICollectVoteResultForAnswerForCurrentQuestion voteResultCollector;

        void Start()
        {
            var dummyNetworkManager = (DummyServerNetworkManager)this.networkManager;
            var dummyServer = (DummyEveryBodyVsTheTeacherServer)this.server;
        
            dummyServer.PresenterId = 1;
            dummyServer.ConnectedMainPlayersConnectionIds = Enumerable.Range(2, 4);

            for (int i = 0; i < dummyServer.ConnectedMainPlayersConnectionIds.Count(); i++)
            {
                var connectionId = dummyServer.ConnectedMainPlayersConnectionIds.Skip(i)
                    .First();
                dummyNetworkManager.SimulateClientConnected(connectionId, "Player " + connectionId);
            }

            this.voteResultCollector.StartCollecting();

            this.CoroutineUtils.WaitForSeconds(1,
                () =>
                    {
                        var answerSelected = new NetworkCommandData("AnswerSelected");
                        var senderConnectionId = dummyServer.ConnectedMainPlayersConnectionIds.First();
                        answerSelected.AddOption("Answer", this.question.CorrectAnswer);                        
                        dummyNetworkManager.FakeReceiveMessage(senderConnectionId, answerSelected.ToString());

                        this.CoroutineUtils.WaitForFrames(1,
                            () =>
                                {
                                    dummyNetworkManager.OnSentDataToClient += (sender, args) =>
                                        {
                                            var command = NetworkCommandData.Parse(args.Message);
                                            if (command.Name == "AnswerSelected" && 
                                                command.Options["Answer"] == this.question.CorrectAnswer)
                                            {
                                                IntegrationTest.Pass();
                                            }
                                        };

                                    dummyNetworkManager.FakeDisconnectPlayer(dummyServer.PresenterId);
                                    dummyServer.PresenterId = 0;

                                    this.CoroutineUtils.WaitForFrames(1,
                                        () =>
                                            {
                                                dummyServer.PresenterId = 1;
                                                dummyNetworkManager.FakeConnectPlayer(1);
                                                var presenterConnectingCommand =
                                                    NetworkCommandData.From<PresenterConnectingCommand>();
                                                dummyNetworkManager.FakeReceiveMessage(1, presenterConnectingCommand.ToString());
                                            });
                                });
                    });
        }
    }
}