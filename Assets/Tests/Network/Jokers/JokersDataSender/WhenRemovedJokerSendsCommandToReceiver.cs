using JokersData = Network.JokersData;
using KalitkoJoker = Jokers.KalitkoJoker;
using NetworkCommandData = Commands.NetworkCommandData;
using Network_JokersDataSender = Network.JokersDataSender;

namespace Tests.Network.Jokers.JokersDataSender
{

    using Interfaces.Network.NetworkManager;

    using Tests.DummyObjects;

    using UnityEngine;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Utils.Unity;

    using Zenject.Source.Usage;

    public class WhenRemovedJokerSendsCommandToReceiver : ExtendedMonoBehaviour
    {
        [Inject]
        private IServerNetworkManager serverNetworkManager;

        [Inject]
        private JokersData jokersData;

        void Start()
        {
            var connectionId = 1;
            var jokersDataSender = new Network_JokersDataSender(this.jokersData, connectionId, this.serverNetworkManager);
            var jokerType = typeof(KalitkoJoker);

            var dummyServerNetworkManager = (DummyServerNetworkManager)this.serverNetworkManager;
            dummyServerNetworkManager.OnSentDataToClient += (sender, args) =>
                {
                    var command = NetworkCommandData.Parse(args.Message);
                    if (command.Name == "Remove" + jokerType.Name)
                    {
                        IntegrationTest.Pass();
                    }
                };

            this.jokersData.AddJoker(jokerType);

            this.CoroutineUtils.WaitForSeconds(0.5f,
                () =>
                    {
                        this.jokersData.RemoveJoker(jokerType);
                    });
        }
    }
}