namespace Controllers.GameController
{

    using Commands;

    using Interfaces.Network.Jokers;
    using Interfaces.Network.NetworkManager;

    using Tests.DummyObjects;

    using UnityEngine;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Zenject.Source.Usage;

    public class SendingSelectAudienceAnswerPoll : MonoBehaviour
    {
        [Inject]
        private IClientNetworkManager clientNetworkManager;

        [Inject]
        private IAnswerPollResultRetriever audienceAnswerPollResultRetriever;

        void Start()
        {
            var dummyClientNetworkManager = (DummyClientNetworkManager)this.clientNetworkManager;
            dummyClientNetworkManager.OnSentToServerMessage += (sender, args) =>
                {
                    var command = NetworkCommandData.Parse(args.Message);
                    if (command.Name == "SelectedHelpFromAudienceJokerRouter")
                    {
                        IntegrationTest.Pass();
                    }
                };
            this.audienceAnswerPollResultRetriever.Activate();
        }
    }
}