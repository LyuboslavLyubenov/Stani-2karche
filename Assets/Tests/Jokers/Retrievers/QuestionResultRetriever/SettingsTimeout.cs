namespace Tests.Jokers.Retrievers.QuestionResultRetriever
{

    using Interfaces;
    using Interfaces.Network.Jokers;

    using UnityEngine;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Zenject.Source.Usage;

    public class SettingsTimeout : MonoBehaviour
    {
        [Inject]
        private ISimpleQuestion question;

        [Inject]
        private IAskClientQuestionResultRetriever askClientQuestionResultRetriever;

        void Start()
        {
            this.askClientQuestionResultRetriever.OnReceiveSettingsTimeout += (sender, args) =>
                {
                    IntegrationTest.Pass();
                };

            this.askClientQuestionResultRetriever.Activate(1);
        }
    }
}