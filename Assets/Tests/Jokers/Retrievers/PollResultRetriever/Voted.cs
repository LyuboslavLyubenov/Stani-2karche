namespace Assets.Tests.Jokers.Retrievers.AudiencePollResultRetriever
{
    using Assets.Scripts.Commands;
    using Assets.Scripts.Commands.Client;
    using Assets.Scripts.Commands.Jokers;
    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Interfaces.Network.Jokers;
    using Assets.Scripts.Interfaces.Network.NetworkManager;
    using Assets.Scripts.Utils.Unity;
    using Assets.Tests.DummyObjects;
    using Assets.UnityTestTools.IntegrationTestsFramework.TestRunner;
    using Assets.Zenject.Source.Usage;
    
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