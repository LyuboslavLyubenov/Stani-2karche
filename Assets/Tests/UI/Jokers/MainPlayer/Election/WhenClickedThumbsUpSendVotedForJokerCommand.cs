using DummyClientNetworkManager = Tests.DummyObjects.DummyClientNetworkManager;
using ExtendedMonoBehaviour = Utils.Unity.ExtendedMonoBehaviour;

namespace Assets.Tests.UI.Jokers.MainPlayer.Election
{

    using Assets.Scripts.Controllers.EveryBodyVsTheTeacher.Jokers.Election.MainPlayer;
    using Assets.Scripts.Extensions.Unity.UI;

    using Commands;

    using Interfaces.Network.NetworkManager;

    using UnityEngine;
    using UnityEngine.UI;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Zenject.Source.Usage;

    public class WhenClickedThumbsUpSendVotedForJokerCommand : ExtendedMonoBehaviour
    {
        [Inject]
        private IClientNetworkManager networkManager;

        [Inject(Id = "ThumbsUpButton")]
        private Button thumbsUpButton;
        
        void Start()
        {
            var jokerElectionUI = GameObject.FindObjectOfType<JokerElectionUIController>().gameObject;
            jokerElectionUI.SetActive(false);

            this.CoroutineUtils.WaitForFrames(1,
                () =>
                    {
                        jokerElectionUI.SetActive(true);

                        this.CoroutineUtils.WaitForFrames(1,
                            () =>
                                {
                                    var dummyNetworkManager = (DummyClientNetworkManager)this.networkManager;
                                    dummyNetworkManager.OnSentToServerMessage += (sender, args) =>
                                        {
                                            var command = NetworkCommandData.Parse(args.Message);
                                            if (command.Name == "PlayerVotedFor")
                                            {
                                                IntegrationTest.Pass();
                                            }
                                        };
                                    this.thumbsUpButton.SimulateClick();
                                });
                    });
        }
    }
}