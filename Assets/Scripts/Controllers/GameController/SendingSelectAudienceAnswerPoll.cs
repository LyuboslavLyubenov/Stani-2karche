namespace Assets.Scripts.Controllers.GameController
{
    using Assets.Scripts.Commands;
    using Assets.Scripts.Interfaces.Network.Jokers;
    using Assets.Scripts.Interfaces.Network.NetworkManager;
    using Assets.Tests.DummyObjects;
    using Assets.UnityTestTools.IntegrationTestsFramework.TestRunner;
    using Assets.Zenject.Source.Usage;

    using UnityEngine;

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