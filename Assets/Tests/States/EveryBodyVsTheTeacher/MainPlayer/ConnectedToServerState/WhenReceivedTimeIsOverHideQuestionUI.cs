using DummyClientNetworkManager = Tests.DummyObjects.DummyClientNetworkManager;
using ExtendedMonoBehaviour = Utils.Unity.ExtendedMonoBehaviour;

namespace Assets.Tests.States.EveryBodyVsTheTeacher.MainPlayer.ConnectedToServerState
{
    using Commands;
    using Commands.Client;

    using Interfaces.Network.NetworkManager;

    using UnityEngine;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Zenject;

    public class WhenReceivedTimeIsOverHideQuestionUI : ExtendedMonoBehaviour
    {
        [Inject]
        private IClientNetworkManager networkManager;

        [Inject(Id="QuestionUI")]
        private GameObject questionUI;

        void Start()
        {
            var dummyClientNetworkManager = (DummyClientNetworkManager)this.networkManager;

            this.questionUI.SetActive(true);

            this.CoroutineUtils.WaitForSeconds(0.5f,
                () =>
                    {
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
                    });
        }
    }
}