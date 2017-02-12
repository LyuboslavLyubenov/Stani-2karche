namespace Assets.Tests.UI.EverybodyVsTheTeacher.MainPlayersContainer
{
    using System.Linq;

    using Assets.Scripts.Utils.Unity;

    using UnityEngine.UI;

    public class AssertOpacityImage : ExtendedMonoBehaviour
    {
        public Image[] Images;

        public float SecondsToWaitBeforeCheck = 3f;

        public float MinOpacity = 0.99f;
        public float MaxOpacity = 1f;

        void Start()
        {
            CoroutineUtils.WaitForSeconds(this.SecondsToWaitBeforeCheck,
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
