using AskClientQuestionResponseCommand = Commands.Client.AskClientQuestionResponseCommand;
using AskClientQuestionSettingsCommand = Commands.Jokers.Settings.AskClientQuestionSettingsCommand;
using NetworkCommandData = Commands.NetworkCommandData;

namespace Tests.Jokers.Retrievers.QuestionResultRetriever
{

    using Interfaces;
    using Interfaces.Network.Jokers;
    using Interfaces.Network.NetworkManager;

    using Tests.DummyObjects;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Utils.Unity;

    using Zenject.Source.Usage;

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