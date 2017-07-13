namespace Assets.Tests.Utils.UnityTimer
{

    using Assets.Scripts.Interfaces.Utils;

    using UnityEngine;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Zenject;

    public class UpdatingEverySecond : MonoBehaviour
    {
        [Inject]
        private IUnityTimer unityTimer;

        void Start()
        {
            this.unityTimer.InvervalInSeconds = 10;
            this.unityTimer.OnSecondPassed += (sender, args) => IntegrationTest.Pass();
            this.unityTimer.StartTimer();
        }
    }
}