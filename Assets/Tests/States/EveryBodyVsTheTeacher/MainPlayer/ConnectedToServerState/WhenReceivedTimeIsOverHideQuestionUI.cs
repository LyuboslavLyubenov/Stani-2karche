using DummyClientNetworkManager = Tests.DummyObjects.DummyClientNetworkManager;

namespace Assets.Tests.States.EveryBodyVsTheTeacher.MainPlayer.ConnectedToServerState
{

    using Commands;
    using Commands.Client;

    using Interfaces.Network.NetworkManager;

    using UnityEngine;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Zenject.Source.Usage;

    public class WhenReceivedTimeIsOverHideQuestionUI : MonoBehaviour
    {
        [Inject]
        private IClientNetworkManager networkManager;

        [Inject(Id="QuestionUI")]
        private GameObject questionUI;

        void Start()
        {
            var dummyClientNetworkManager = (DummyClientNetworkManager)this.networkManager;
            var timeoutCommand = NetworkCommandData.From<AnswerTimeoutCommand>();
            dummyClientNetworkManager.FakeReceiveMessage(timeoutCommand.ToString());

            if (!this.questionUI.activeSelf)
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