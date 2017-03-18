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