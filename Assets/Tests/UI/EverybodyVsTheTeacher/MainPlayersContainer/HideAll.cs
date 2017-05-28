using ExtendedMonoBehaviour = Utils.Unity.ExtendedMonoBehaviour;

namespace Assets.Tests.UI.EverybodyVsTheTeacher.MainPlayersContainer
{
    using Interfaces.Controllers;
    
    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Zenject.Source.Usage;

    public class HideAll : ExtendedMonoBehaviour
    {
        [Inject]
        private IMainPlayersContainerUIController mainPlayersContainerUIController;

        void Start()
        {
            this.mainPlayersContainerUIController.ShowMainPlayer(1, "Main player 1");
            this.mainPlayersContainerUIController.ShowMainPlayer(2, "Main player 2");

            this.CoroutineUtils.WaitForFrames(1,
                () =>
                    {
                        this.mainPlayersContainerUIController.HideAll();

                        this.CoroutineUtils.WaitForFrames(1,
                            () =>
                                {
                                    if (!this.mainPlayersContainerUIController.IsOnScreen(1) &&
                                        !this.mainPlayersContainerUIController.IsOnScreen(2))
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