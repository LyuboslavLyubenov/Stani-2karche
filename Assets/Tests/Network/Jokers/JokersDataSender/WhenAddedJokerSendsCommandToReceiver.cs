using JokersData = Network.JokersData;
using NetworkCommandDataClass = Commands.NetworkCommandData;
using Network_JokersDataSender = Network.JokersDataSender;
using Jokers.EveryBodyVsTheTeacher.Kalitko;

namespace Tests.Network.Jokers.JokersDataSender
{

    using Interfaces.Network.NetworkManager;

    using Tests.DummyObjects;

    using UnityEngine;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Zenject;

    public class WhenAddedJokerSendsCommandToReceiver : MonoBehaviour
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
                    var command = NetworkCommandDataClass.Parse(args.Message);
                    if (command.Name == "Add" + jokerType.Name)
                    {
                        IntegrationTest.Pass();
                    }
                };

            this.jokersData.AddJoker(jokerType);
        }
    }

}