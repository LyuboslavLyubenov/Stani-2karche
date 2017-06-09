using DummyClientNetworkManager = Tests.DummyObjects.DummyClientNetworkManager;
using ExtendedMonoBehaviour = Utils.Unity.ExtendedMonoBehaviour;

namespace Assets.Tests.States.EveryBodyVsTheTeacher.Audience.ConnectedToServerState
{

    using Assets.Scripts.Extensions.Unity.UI;
    using Assets.Scripts.Interfaces;

    using Commands;
    using Commands.Client;
    
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

        [Inject]
        private IState state;
        
        void Start()
        {
            var dummyClientNetworkManager = (DummyClientNetworkManager)this.networkManager;
            var loadQuestionCommand = NetworkCommandData.From<LoadQuestionCommand>();
            var questionJSON = JsonUtility.ToJson(this.question.Serialize());
            loadQuestionCommand.AddOption("QuestionJSON", questionJSON);
            loadQuestionCommand.AddOption("TimeToAnswer", "10");
            dummyClientNetworkManager.FakeReceiveMessage(loadQuestionCommand.ToString());

            this.CoroutineUtils.WaitForSeconds(1f,
                () =>
                    {
                        var answerButton = this.questionUI.GetComponentInChildren<Button>();
                        var answerText = answerButton.GetComponentInChildren<Text>()
                            .text;

                        dummyClientNetworkManager.OnSentToServerMessage += (sender, args) =>
                            {
                                var command = NetworkCommandData.Parse(args.Message);
                                if (command.Name == "AnswerSelected" && command.Options["Answer"] == answerText)
                                {
                                    IntegrationTest.Pass();
                                }
                            };

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