using AnswerPollResultCommand = Commands.Client.AnswerPollResultCommand;
using AnswerPollSettingsCommand = Commands.Jokers.Settings.AnswerPollSettingsCommand;
using NetworkCommandData = Commands.NetworkCommandData;

namespace Tests.Jokers.Retrievers.PollResultRetriever
{

    using Interfaces;
    using Interfaces.Network.Jokers;
    using Interfaces.Network.NetworkManager;

    using Tests.DummyObjects;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Utils.Unity;

    using Zenject;

    public class Voted : ExtendedMonoBehaviour
    {
        [Inject]
        private ISimpleQuestion question;

        [Inject]
        private IClientNetworkManager clientNetworkManager;

        [Inject]
        private IAnswerPollResultRetriever audienceAnswerPollResultRetriever;

        void Start()
        {
            var dummyClientNetworkManager = (DummyClientNetworkManager)this.clientNetworkManager;
            var timeToAnswerInSeconds = 5;
            this.audienceAnswerPollResultRetriever.OnVoted += (sender, args) =>
                {
                    if (args.AnswersVotes[this.question.Answers[0]] == 1)
                    {
                        this.audienceAnswerPollResultRetriever.Dispose();
                        IntegrationTest.Pass();
                    }
                };
            this.audienceAnswerPollResultRetriever.Activate();
            this.CoroutineUtils.WaitForSeconds(0.5f,
                () =>
                    {
                        var settingsCommand = NetworkCommandData.From<AnswerPollSettingsCommand>();
                        settingsCommand.AddOption("TimeToAnswerInSeconds", timeToAnswerInSeconds.ToString());
                        dummyClientNetworkManager.FakeReceiveMessage(settingsCommand.ToString());

                        this.CoroutineUtils.WaitForSeconds(0.5f,
                            () =>
                                {
                                    var answerPollResultCommand = NetworkCommandData.From<AnswerPollResultCommand>();
                                    answerPollResultCommand.AddOption(this.question.Answers[0], "1");
                                    dummyClientNetworkManager.FakeReceiveMessage(answerPollResultCommand.ToString());
                                });
                    });
        }
    }
}