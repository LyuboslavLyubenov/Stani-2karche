using DummyClientNetworkManager = Tests.DummyObjects.DummyClientNetworkManager;

namespace Assets.Tests.States.EveryBodyVsTheTeacher.MainPlayer.ConnectedToServerState
{
    using Commands;
    using Commands.EveryBodyVsTheTeacher;

    using Interfaces.Network.NetworkManager;

    using UnityEngine;
    using UnityEngine.UI;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Zenject;

    public class ActivateGameStartButtonIfThereAreEnoughPlayers : MonoBehaviour
    {
        [Inject]
        private IClientNetworkManager networkManager;

        [Inject(Id="GameStartButton")]
        private Button gameStartButton;

        void Start()
        {
            var dummyClientNetworkManager = (DummyClientNetworkManager)this.networkManager;
            var enoughPlayersCommand = NetworkCommandData.From<EnoughPlayersToStartGameCommand>();
            dummyClientNetworkManager.FakeReceiveMessage(enoughPlayersCommand.ToString());

            if (this.gameStartButton.gameObject.activeSelf)
            {
                IntegrationTest.Pass();
            }
            else
            {
                IntegrationTest.Fail();
            }
        }
    }
}