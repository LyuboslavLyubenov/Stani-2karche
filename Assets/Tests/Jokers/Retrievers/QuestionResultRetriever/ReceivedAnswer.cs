namespace Tests.Jokers.Retrievers.QuestionResultRetriever
{
    using Assets.Scripts.Commands;
    using Assets.Scripts.Commands.Client;
    using Assets.Scripts.Commands.Jokers;
    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Interfaces.Network.NetworkManager;
    using Assets.Scripts.Utils.Unity;
    using Assets.Tests.DummyObjects;
    using Assets.UnityTestTools.IntegrationTestsFramework.TestRunner;
    using Assets.Zenject.Source.Usage;

    using Interfaces.Network.Jokers;

    public class ReceivedAnswer : ExtendedMonoBehaviour
    {
        [Inject]
        private ISimpleQuestion question;

        [Inject]
        private IAskClientQuestionResultRetriever askClientQuestionResultRetriever;

        [Inject]
        private IClientNetworkManager networkManager;

        void Start()
        {
            var timeToAnswerInSeconds = 5;
            var username = "Ivan";
            var answer = this.question.Answers[0];

            this.askClientQuestionResultRetriever.OnReceivedAnswer += (sender, args) =>
                {
                    this.askClientQuestionResultRetriever.Dispose();

                    if (args.Username == username && args.Answer == answer)
                    {
                        IntegrationTest.Pass();
                    }
                };

            this.askClientQuestionResultRetriever.Activate(1);

            this.CoroutineUtils.WaitForSeconds(1f,
                () =>
                    {
                        var dummyClientNetworkManager = (DummyClientNetworkManager)this.networkManager;
                        var settingsCommand = NetworkCommandData.From<AskClientQuestionSettingsCommand>();
                        settingsCommand.AddOption("TimeToAnswerInSeconds", timeToAnswerInSeconds.ToString());
                        dummyClientNetworkManager.FakeReceiveMessage(settingsCommand.ToString());

                        this.CoroutineUtils.WaitForSeconds(1f,
                            () =>
                                {
                                    var answerSelected = NetworkCommandData.From<AskClientQuestionResponseCommand>();
                                    answerSelected.AddOption("Username", username);
                                    answerSelected.AddOption("Answer", answer);
                                    dummyClientNetworkManager.FakeReceiveMessage(answerSelected.ToString());
                                });
                    });
        }
        
        void OnDisable()
        {
            this.askClientQuestionResultRetriever.Dispose();
        }
    }

}