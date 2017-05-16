using DummyClientNetworkManager = Tests.DummyObjects.DummyClientNetworkManager;
using ExtendedMonoBehaviour = Utils.Unity.ExtendedMonoBehaviour;

namespace Assets.Tests.States.EveryBodyVsTheTeacher.MainPlayer.ConnectedToServerState
{
    using Assets.Scripts.Commands.EveryBodyVsTheTeacher.Shared;

    using Commands;

    using Interfaces.Network.NetworkManager;

    using UnityEngine;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Zenject.Source.Usage;

    public class WhenGameStartedActivatePlayingUI : ExtendedMonoBehaviour
    {
        [Inject]
        private IClientNetworkManager networkManager;

        [Inject(Id="PlayingUI")]
        private GameObject playingUI;

        void Start()
        {
            this.playingUI.SetActive(false);

            this.CoroutineUtils.WaitForFrames(1,
                () =>
                    {
                        var dummyClientNetworkManager = (DummyClientNetworkManager)this.networkManager;
                        var gameStartedCommand = NetworkCommandData.From<GameStartedCommand>();
                        dummyClientNetworkManager.FakeReceiveMessage(gameStartedCommand.ToString());

                        if (this.playingUI.activeSelf)
                        {
                            IntegrationTest.Pass();
                        }
                        else
                        {
                            IntegrationTest.Fail();
                        }
                    });
        }
    }
}