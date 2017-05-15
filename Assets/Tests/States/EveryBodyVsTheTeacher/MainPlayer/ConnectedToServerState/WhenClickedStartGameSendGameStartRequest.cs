using DummyClientNetworkManager = Tests.DummyObjects.DummyClientNetworkManager;

namespace Assets.Tests.States.EveryBodyVsTheTeacher.MainPlayer.ConnectedToServerState
{
    using Commands;
    using Commands.EveryBodyVsTheTeacher.PlayersConnectingState;

    using Extensions.Unity.UI;

    using Interfaces.Network.NetworkManager;

    using UnityEngine;
    using UnityEngine.UI;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Zenject.Source.Usage;

    public class WhenClickedStartGameSendGameStartRequest : MonoBehaviour
    {
        [Inject]
        private IClientNetworkManager networkManager;

        [Inject]
        private Button gameStartButton;

        void Start()
        {
            var dummmyNetworkManager = (DummyClientNetworkManager)this.networkManager;
            dummmyNetworkManager.OnSentToServerMessage += (sender, args) =>
                {
                    var command = NetworkCommandData.Parse(args.Message);
                    if (command.Name == typeof(MainPlayerRequestedGameStartCommand).Name.Replace("Command", ""))
                    {
                        IntegrationTest.Pass();
                    }
                    else
                    {
                        IntegrationTest.Fail();
                    }
                }; 

            this.gameStartButton.SimulateClick();
        }
    }
}