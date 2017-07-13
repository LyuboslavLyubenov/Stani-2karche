using ExtendedMonoBehaviour = Utils.Unity.ExtendedMonoBehaviour;

namespace Assets.Tests.Utils.UnityTimer
{

    using Assets.Scripts.Interfaces.Utils;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Zenject;

    public class Stoping : ExtendedMonoBehaviour
    {
        [Inject]
        private IUnityTimer timer;

        void Start()
        {
            this.timer.InvervalInSeconds = 3;
            this.timer.StartTimer();
            this.CoroutineUtils.WaitForSeconds(1.1f,
                () =>
                    {
                        this.timer.StopTimer();
                        this.timer.OnSecondPassed += (sender, args) => IntegrationTest.Fail();
                        this.timer.OnFinished += (sender, args) => IntegrationTest.Fail();
                    });
            this.CoroutineUtils.WaitForSeconds(4f,
                () =>
                {
                    if (!this.timer.Running)
                    {
                        IntegrationTest.Pass();
                    }
                });
        }
    }
}