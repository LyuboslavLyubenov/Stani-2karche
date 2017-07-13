using DummyClientNetworkManager = Tests.DummyObjects.DummyClientNetworkManager;

namespace Assets.Tests.States.EveryBodyVsTheTeacher.MainPlayer.ConnectedToServerState
{

    using Assets.Scripts.Extensions.Unity.UI;

    using Commands;
    using Commands.EveryBodyVsTheTeacher.PlayersConnectingState;
    
    using Interfaces.Network.NetworkManager;

    using UnityEngine;
    using UnityEngine.UI;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Zenject;

    public class WhenClickedStartGameSendGameStartRequest : MonoBehaviour
    {
        [Inject]
        private IClientNetworkManager networkManager;

        [Inject(Id="GameStartButton")]
        private Button gameStartButton;

        void Start()
        {
            this.gameStartButton.gameObject.SetActive(true);

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