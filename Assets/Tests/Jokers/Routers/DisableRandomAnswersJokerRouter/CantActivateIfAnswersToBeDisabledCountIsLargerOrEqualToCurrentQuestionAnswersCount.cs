using Commands;
using Assets.Tests.Utils;
using Commands.Jokers;
using UnityTestTools.IntegrationTestsFramework.TestRunner;
using Interfaces;

namespace Tests.Jokers.Routers.DisableRandomAnswersRouter
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    using Zenject;
    using Interfaces.Network.NetworkManager;
    using Interfaces.Network.Jokers.Routers;
    using Tests.DummyObjects;

    public class CantActivateIfAnswersToBeDisabledCountIsLargerOrEqualToCurrentQuestionAnswersCount : MonoBehaviour
    {
        [Inject]
        private IDisableRandomAnswersRouter router;

        [Inject]
        private ISimpleQuestion question;

        void Start()
        {

            this.router.OnError += (object sender, System.UnhandledExceptionEventArgs args) => IntegrationTest.Pass();
            this.router.Activate(question.Answers.Length + 1, 1);
        }
    }
}