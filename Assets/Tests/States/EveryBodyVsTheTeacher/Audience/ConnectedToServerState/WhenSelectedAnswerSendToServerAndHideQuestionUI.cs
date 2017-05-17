using DummyClientNetworkManager = Tests.DummyObjects.DummyClientNetworkManager;
using ExtendedMonoBehaviour = Utils.Unity.ExtendedMonoBehaviour;

namespace Assets.Tests.States.EveryBodyVsTheTeacher.Audience.ConnectedToServerState
{
    using Commands;
    using Commands.Client;

    using Extensions.Unity.UI;

    using Interfaces;
    using Interfaces.Network.NetworkManager;

    using UnityEngine;
    using UnityEngine.UI;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Zenject.Source.Usage;

    public class WhenSelectedAnswerSendToServerAndHideQuestionUI : ExtendedMonoBehaviour
    {
        [Inject]
        private IClientNetworkManager networkManager;

        [Inject]
        private ISimpleQuestion question;

        [Inject(Id="QuestionUI")]
        private GameObject questionUI;
        
        void Start()
        {
            var dummyClientNetworkManager = (DummyClientNetworkManager)this.networkManager;
            var loadQuestionCommand = NetworkCommandData.From<LoadQuestionCommand>();
            var questionJSON = JsonUtility.ToJson(this.question.Serialize());
            loadQuestionCommand.AddOption("QuestionJSON", questionJSON);
            loadQuestionCommand.AddOption("TimeToAnswerInSeconds", "10");
            dummyClientNetworkManager.FakeReceiveMessage(loadQuestionCommand.ToString());

            this.CoroutineUtils.WaitForFrames(1,
                () =>
                    {
                        var answerButton = this.questionUI.GetComponentInChildren<Button>();
                        answerButton.SimulateClick();

                        this.CoroutineUtils.WaitForFrames(1,
                            () =>
                                {
                                    if (!this.questionUI.activeSelf)
                                    {
                                        IntegrationTest.Pass();
                                    }
                                    else
                                    {
                                        IntegrationTest.Fail();
                                    }
                                });
                    });
        }
    }
}