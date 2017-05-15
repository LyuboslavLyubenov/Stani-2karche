using DummyClientNetworkManager = Tests.DummyObjects.DummyClientNetworkManager;

namespace Assets.Tests.States.EveryBodyVsTheTeacher.MainPlayer.ConnectedToServerState
{
    using Commands;

    using Extensions.Unity.UI;

    using Interfaces.Network.NetworkManager;

    using UnityEngine;
    using UnityEngine.UI;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Zenject.Source.Usage;

    public class WhenClickedOnAnswerSendToServerAndDontAllowToSendAgain : MonoBehaviour
    {
        [Inject]
        private IClientNetworkManager networkManager;

        [Inject(Id="AnswerButton")]
        private Button answerButton;
    
        [Inject(Id="SelectedAnswer")]
        private string selectedAnswer;

        [Inject(Id="QuestionUI")]
        private GameObject questionUI;
        void Start()
        {
            var dummyClientNetworkManager = (DummyClientNetworkManager)this.networkManager;
            dummyClientNetworkManager.OnSentToServerMessage += (sender, args) =>
                {
                    var command = NetworkCommandData.Parse(args.Message);
                    var correctCommandSent = 
                            command.Name == "AnswerSelected" && 
                            command.Options["Answer"] == this.selectedAnswer;

                    if (!this.questionUI.activeSelf && correctCommandSent)
                    {
                        IntegrationTest.Pass();
                    }
                    else
                    {
                        IntegrationTest.Fail();
                    }
                };

            this.answerButton.SimulateClick();
        }
    }
}