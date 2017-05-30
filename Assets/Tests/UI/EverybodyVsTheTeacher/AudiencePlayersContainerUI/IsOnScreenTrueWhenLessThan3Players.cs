using ExtendedMonoBehaviour = Utils.Unity.ExtendedMonoBehaviour;

namespace Assets.Tests.UI.EverybodyVsTheTeacher.AudiencePlayersContainerUI
{

    using Interfaces.Controllers;
    
    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Zenject.Source.Usage;

    public class IsOnScreenTrueWhenLessThan3Players : ExtendedMonoBehaviour
    {
        [Inject]
        private IAudiencePlayersContainerUIController audiencePlayersContainerUiController;

        void Start()
        {
            this.audiencePlayersContainerUiController.ShowAudiencePlayer(1, "Player 1");

            this.CoroutineUtils.WaitForFrames(1,
                () =>
                    {
                        if (this.audiencePlayersContainerUiController.IsOnScreen(1))
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