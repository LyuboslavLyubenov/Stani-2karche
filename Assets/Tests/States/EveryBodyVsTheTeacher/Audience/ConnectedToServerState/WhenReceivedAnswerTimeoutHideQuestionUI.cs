using DummyClientNetworkManager = Tests.DummyObjects.DummyClientNetworkManager;
using ExtendedMonoBehaviour = Utils.Unity.ExtendedMonoBehaviour;

namespace Assets.Tests.States.EveryBodyVsTheTeacher.Audience.ConnectedToServerState
{
    using Commands;
    using Commands.Client;

    using Interfaces;
    using Interfaces.Network.NetworkManager;

    using UnityEngine;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Zenject;

    public class WhenReceivedAnswerTimeoutHideQuestionUI : ExtendedMonoBehaviour
    {
        [Inject]
        private IClientNetworkManager networkManager;

        [Inject(Id="QuestionUI")]
        private GameObject questionUI;

        [Inject]
        private ISimpleQuestion question;

        void Start()
        {
            var dummyClientNetworkManager = (DummyClientNetworkManager)this.networkManager;
            var loadQuestionCommand = NetworkCommandData.From<LoadQuestionCommand>();
            var questionJSON = JsonUtility.ToJson(this.question.Serialize());
            loadQuestionCommand.AddOption("QuestionJSON", questionJSON);
            loadQuestionCommand.AddOption("TimeToAnswer", "10");
            dummyClientNetworkManager.FakeReceiveMessage(loadQuestionCommand.ToString());

            this.CoroutineUtils.WaitForFrames(1,
                () =>
                    {
                        var answerTimeoutCommand = NetworkCommandData.From<AnswerTimeoutCommand>();
                        dummyClientNetworkManager.FakeReceiveMessage(answerTimeoutCommand.ToString());

                        if (!this.questionUI.activeSelf)
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