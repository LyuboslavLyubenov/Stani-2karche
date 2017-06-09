using NetworkCommandData = Commands.NetworkCommandData;

namespace Tests.States.RoundsSwitcherEventsNotifier
{

    using Assets.Scripts.Interfaces.States.EveryBodyVsTheTeacher.Server;
    using Assets.Scripts.Network.EveryBodyVsTheTeacher;

    using Interfaces.Network.NetworkManager;

    using Tests.DummyObjects;

    using UnityEngine;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Zenject.Source.Usage;

    public class NoMoreRounds : MonoBehaviour
    {
        [Inject]
        private IServerNetworkManager networkManager;

        [Inject]
        private IRoundsSwitcher roundsSwitcher;

        [Inject]
        private RoundsSwitcherEventsNotifier notifier;

        [Inject]
        private int receiverConnectionId;

        void Start()
        {
            var dummyServerNetworkManager = (DummyServerNetworkManager)this.networkManager;
            dummyServerNetworkManager.OnSentDataToClient += (sender, args) =>
                {
                    var command = NetworkCommandData.Parse(args.Message);
                    if (command.Name == "NoMoreRounds" && args.ConnectionId == this.receiverConnectionId)
                    {
                        IntegrationTest.Pass();
                    }
                };

            this.roundsSwitcher.SwitchToNextRound();
            this.roundsSwitcher.SwitchToNextRound();
        }
    }

}