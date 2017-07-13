using DummyClientNetworkManager = Tests.DummyObjects.DummyClientNetworkManager;
using ExtendedMonoBehaviour = Utils.Unity.ExtendedMonoBehaviour;

namespace Assets.Tests.States.EveryBodyVsTheTeacher.MainPlayer.ConnectedToServerState
{
    using Commands;
    
    using Assets.Scripts.Extensions.Unity.UI;

    using Interfaces;
    using Interfaces.Controllers;
    using Interfaces.Network.NetworkManager;

    using UnityEngine;
    using UnityEngine.UI;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Zenject;

    public class WhenClickedOnAnswerSendToServerAndHideQuestionUI : ExtendedMonoBehaviour
    {
        [Inject]
        private IClientNetworkManager networkManager;
        
        private Button answerButton
        {
            get
            {
                return this.questionUI.GetComponentInChildren<Button>();
            }
        }
    
        private string selectedAnswer
        {
            get
            {
                return this.answerButton.GetComponentInChildren<Text>()
                    .text;
            }
        }

        [Inject(Id="QuestionUI")]
        private GameObject questionUI;

        [Inject]
        private IQuestionUIController questionUIController;

        [Inject]
        private ISimpleQuestion question;

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

            this.questionUI.SetActive(true);
            this.questionUIController.LoadQuestion(this.question);

            this.CoroutineUtils.WaitForSeconds(2f, () => this.answerButton.SimulateClick());
        }
    }
}