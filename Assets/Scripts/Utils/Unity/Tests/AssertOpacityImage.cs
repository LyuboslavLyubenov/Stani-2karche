namespace Utils.Unity.Tests
{

    using System.Linq;

    using UnityEngine.UI;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    public class AssertOpacityImage : ExtendedMonoBehaviour
    {
        public Image[] Images;

        public float SecondsToWaitBeforeCheck = 3f;

        public float MinOpacity = 0.99f;
        public float MaxOpacity = 1f;

        void Start()
        {
            this.CoroutineUtils.WaitForSeconds(this.SecondsToWaitBeforeCheck,
                () =>
                    {
                        var areAllWithMaximumOpacity = this.Images.All(i => i.color.a >= this.MinOpacity && i.color.a <= this.MaxOpacity);

                        if (areAllWithMaximumOpacity)
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
