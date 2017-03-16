namespace Tests.Jokers.Retrievers.QuestionResultRetriever
{

    using Assets.Scripts.Interfaces;
    using Assets.UnityTestTools.IntegrationTestsFramework.TestRunner;
    using Assets.Zenject.Source.Usage;

    using Interfaces.Network.Jokers;

    using UnityEngine;

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