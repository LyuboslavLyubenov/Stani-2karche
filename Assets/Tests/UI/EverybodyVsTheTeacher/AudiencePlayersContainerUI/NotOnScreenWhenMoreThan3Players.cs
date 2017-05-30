using ExtendedMonoBehaviour = Utils.Unity.ExtendedMonoBehaviour;

namespace Assets.Tests.UI.EverybodyVsTheTeacher.AudiencePlayersContainerUI
{

    using System.Collections;

    using Interfaces.Controllers;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Zenject.Source.Usage;

    public class NotOnScreenWhenMoreThan3Players : ExtendedMonoBehaviour
    {
        [Inject]
        private IAudiencePlayersContainerUIController audiencePlayersContainerUiController;

        void Start()
        {
            this.StartCoroutine(this.Test());
        }

        private IEnumerator Test()
        {
            yield return null;

            for (int i = 1; i <= 5; i++)
            {
                this.audiencePlayersContainerUiController.ShowAudiencePlayer(i, "Player " + i);
                yield return null;
            }

            yield return null;

            if (!this.audiencePlayersContainerUiController.IsOnScreen(1))
            {
                IntegrationTest.Pass();
            }
            else
            {
                IntegrationTest.Fail();
            }
        }
    }
}