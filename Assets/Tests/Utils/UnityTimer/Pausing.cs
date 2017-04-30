using ExtendedMonoBehaviour = Utils.Unity.ExtendedMonoBehaviour;

namespace Assets.Tests.Utils.UnityTimer
{

    using System;

    using Assets.Scripts.Interfaces.Utils;

    using UnityEngine;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Zenject.Source.Usage;

    public class Pausing : ExtendedMonoBehaviour
    {
        [Inject]
        private IUnityTimer unityTimer;

        void Start()
        {
            this.unityTimer.InvervalInSeconds = 2;
            this.unityTimer.StartTimer();

            this.CoroutineUtils.WaitForSeconds(1f,
                () =>
                {
                    this.unityTimer.Paused = true;
                    this.unityTimer.OnSecondPassed += this.OnSecondPassedMustFail;

                    this.CoroutineUtils.WaitForSeconds(2,
                        () =>
                        {
                            this.unityTimer.Paused = false;
                            this.unityTimer.OnSecondPassed -= this.OnSecondPassedMustFail;
                            this.unityTimer.OnSecondPassed += this.OnSecondPassedMustPass;
                        });
                });
        }

        private void OnSecondPassedMustFail(object sender, EventArgs args)
        {
            IntegrationTest.Fail();
        }

        private void OnSecondPassedMustPass(object sender, EventArgs args)
        {
            IntegrationTest.Pass();
        }
    }
}