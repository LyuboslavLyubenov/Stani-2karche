namespace Tests.Jokers.Retrievers.QuestionResultRetriever
{

    using Assets.Scripts.Commands;
    using Assets.Scripts.Commands.Jokers;
    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Interfaces.Network.NetworkManager;
    using Assets.Scripts.Utils.Unity;
    using Assets.Tests.DummyObjects;
    using Assets.UnityTestTools.IntegrationTestsFramework.TestRunner;
    using Assets.Zenject.Source.Usage;

    using Interfaces.Network.Jokers;

    public class ReceiveAnswerTimeout : ExtendedMonoBehaviour
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
            this.askClientQuestionResultRetriever.OnReceiveAnswerTimeout += (sender, args) =>
                {
                    this.askClientQuestionResultRetriever.Dispose();
                    IntegrationTest.Pass();
                };

            this.askClientQuestionResultRetriever.Activate(1);

            this.CoroutineUtils.WaitForSeconds(1f,
                () =>
                    {
                        var dummyClientNetworkManager = (DummyClientNetworkManager)this.networkManager;
                        var settingsCommand = NetworkCommandData.From<AskClientQuestionSettingsCommand>();
                        settingsCommand.AddOption("TimeToAnswerInSeconds", timeToAnswerInSeconds.ToString());
                        dummyClientNetworkManager.FakeReceiveMessage(settingsCommand.ToString());
                    });
        }


        void OnDisable()
        {
            this.askClientQuestionResultRetriever.Dispose();
        }
    }

}