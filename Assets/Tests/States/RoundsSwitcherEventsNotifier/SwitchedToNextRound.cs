using DummyServerNetworkManager = Tests.DummyObjects.DummyServerNetworkManager;
using NetworkCommandData = Commands.NetworkCommandData;

namespace Tests.States.RoundsSwitcherEventsNotifier
{
    using Assets.Scripts.Interfaces.States.EveryBodyVsTheTeacher.Server;
    using Assets.Scripts.Network.EveryBodyVsTheTeacher;

    using Interfaces.Network.NetworkManager;

    using UnityEngine;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Zenject;

    public class SwitchedToNextRound : MonoBehaviour
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
                    if (command.Name == "SwitchedToNextRound" && args.ConnectionId == this.receiverConnectionId)
                    {
                        IntegrationTest.Pass();
                    }
                };

            this.roundsSwitcher.SwitchToNextRound();
        }
    }
}