namespace Tests.Jokers.Retrievers.PollResultRetriever
{

    using Interfaces.Network.Jokers;
    using Interfaces.Network.NetworkManager;

    using UnityEngine;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Zenject.Source.Usage;

    public class ReceiveSettingsTimeout : MonoBehaviour
    {

        [Inject]
        private IClientNetworkManager clientNetworkManager;

        [Inject]
        private IAnswerPollResultRetriever audienceAnswerPollResultRetriever;

        void Start()
        {
            this.audienceAnswerPollResultRetriever.OnReceivedSettingsTimeout += (sender, args) =>
                {
                    this.audienceAnswerPollResultRetriever.Dispose();
                    IntegrationTest.Pass();
                };
            this.audienceAnswerPollResultRetriever.Activate();
        }
    }
}
