namespace Assets.Tests.Jokers.Retrievers.AudiencePollResultRetriever
{

    using Assets.Scripts.Interfaces.Network.Jokers;
    using Assets.Scripts.Interfaces.Network.NetworkManager;
    using Assets.UnityTestTools.IntegrationTestsFramework.TestRunner;
    using Assets.Zenject.Source.Usage;

    using UnityEngine;

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
