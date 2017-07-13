
namespace Assets.Tests.Utils.UnityTimer
{

    using Assets.Scripts.Interfaces.Utils;

    using UnityEngine;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Zenject;

    public class FinishedAfterSecondsSet : MonoBehaviour
    {
        [Inject]
        private IUnityTimer timer;

        void Start()
        {
            this.timer.InvervalInSeconds = 2;
            this.timer.OnFinished += (sender, args) => IntegrationTest.Pass();
            this.timer.StartTimer();
        }
    }
}