namespace Assets.Tests.UI.EverybodyVsTheTeacher.MainPlayersContainer
{

    using Assets.Scripts.Utils.Unity;
    using Assets.UnityTestTools.IntegrationTestsFramework.TestRunner;

    using UnityEngine;
    using UnityEngine.UI.Extensions;

    public class AssertOutline : ExtendedMonoBehaviour
    {
        public GameObject Obj;
        public bool ShouldBeActivate = true;

        public float CheckAfterTimeInSeconds = 1f;

        // Use this for initialization
        void Start()
        {
            this.CoroutineUtils.WaitForSeconds(this.CheckAfterTimeInSeconds, this.AssertActiveOutline);
        }

        private void AssertActiveOutline()
        {
            var outlineComponent = this.Obj.GetComponent<NicerOutline>();

            if (outlineComponent == null || outlineComponent.enabled != this.ShouldBeActivate)
            {
                IntegrationTest.Fail();
            }
            else
            {
                IntegrationTest.Pass();
            }
        }
    }

}
