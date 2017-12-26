using System;
using UnityEngine;
using Zenject;
using Assets.Scripts.Interfaces;
using UnityTestTools.IntegrationTestsFramework.TestRunner;
using Utils;

namespace Tests.Jokers.DisableRandomAnswers
{

    public class WhenActivatedAndTimeForReceiveSettingsIsOverDeactivateJoker : MonoBehaviour
    {
        [Inject]
        private IJoker joker;

        void Start()
        {
            var threadUtils = ThreadUtils.Instance;

            this.joker.Activate();

            this.joker.OnError += (object sender, UnhandledExceptionEventArgs args) =>
            {
                if (!this.joker.Activated && ((TimeoutException)args.ExceptionObject) != null)
                {
                    IntegrationTest.Pass();
                }
            };
        }
    }
}
