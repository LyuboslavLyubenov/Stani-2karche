using Zenject;
using Interfaces.Network.Jokers.Routers;
using UnityTestTools.IntegrationTestsFramework.TestRunner;

namespace Tests.Jokers.Routers.DisableRandomAnswersRouter 
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class CantActivateIfAnswersToDisableAreNotPositiveNumber : MonoBehaviour 
    {
        [Inject]
        private IDisableRandomAnswersRouter router;

        void Start () 
        {
            this.router.OnError += (object sender, System.UnhandledExceptionEventArgs args) => IntegrationTest.Pass();
            this.router.Activate(-1, 1);
        }
    }
}