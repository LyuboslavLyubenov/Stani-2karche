using ExtendedMonoBehaviour = Utils.Unity.ExtendedMonoBehaviour;

namespace Assets.Tests.UI.EverybodyVsTheTeacher.AudiencePlayersContainerUI
{
    using Interfaces.Controllers;
    
    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Zenject.Source.Usage;

    public class HideAll : ExtendedMonoBehaviour
    {
        [Inject]
        private IAudiencePlayersContainerUIController audiencePlayersContainerUIController;

        void Start()
        {
            this.audiencePlayersContainerUIController.ShowAudiencePlayer(1, "Audience player 1");
            this.audiencePlayersContainerUIController.ShowAudiencePlayer(2, "Audience player 2");

            this.CoroutineUtils.WaitForFrames(1,
                () =>
                    {
                        this.audiencePlayersContainerUIController.HideAll();
                        
                        this.CoroutineUtils.WaitForFrames(1,
                            () =>
                                {
                                    if (!this.audiencePlayersContainerUIController.IsOnScreen(1) &&
                                        !this.audiencePlayersContainerUIController.IsOnScreen(2))
                                    {
                                        IntegrationTest.Pass();
                                    }
                                    else
                                    {
                                        IntegrationTest.Fail();
                                    }
                                });
                    });
        }
    }
}