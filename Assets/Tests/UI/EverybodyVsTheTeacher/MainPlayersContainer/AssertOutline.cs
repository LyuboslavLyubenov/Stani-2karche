namespace Tests.UI.EverybodyVsTheTeacher.MainPlayersContainer
{

    using UnityEngine;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Utils.Unity;

    public class AssertOutline : ExtendedMonoBehaviour
    {
        public GameObject Obj;
        public bool ShouldBeActive = true;

        public float CheckAfterTimeInSeconds = 1f;

        // Use this for initialization
        void Start()
        {
            this.CoroutineUtils.WaitForSeconds(this.CheckAfterTimeInSeconds, this.AssertActiveOutline);
        }

        private void AssertActiveOutline()
        {
            var outlineComponent = this.Obj.GetComponent<NicerOutline>();

            if (outlineComponent == null || outlineComponent.enabled != this.ShouldBeActive)
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
