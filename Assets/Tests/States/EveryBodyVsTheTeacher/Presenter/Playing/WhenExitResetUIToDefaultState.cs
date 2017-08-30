using ExtendedMonoBehaviour = Utils.Unity.ExtendedMonoBehaviour;

namespace Assets.Tests.States.EveryBodyVsTheTeacher.Presenter.Playing
{
    using Assets.Scripts.States.EveryBodyVsTheTeacher.Presenter;

    using Interfaces.Controllers;

    using StateMachine;

    using UnityEngine;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Zenject;

    public class WhenExitResetUIToDefaultState : ExtendedMonoBehaviour
    {
        [Inject]
        private IMainPlayersContainerUIController mainPlayersContainerUiController;

        [Inject]
        private IAudiencePlayersContainerUIController audiencePlayersContainerUiController;

        [Inject(Id = "PlayingUI")]
        private GameObject playingUI;

        [Inject]
        private PlayingState playingState;

        private void AddMainPlayers(int mainPlayersCount)
        {
            for (int i = 0; i < mainPlayersCount; i++)
            {
                var connectionId = i + 1;
                var username = "Main player " + i;
                this.mainPlayersContainerUiController.ShowMainPlayer(connectionId, username);
            }
        }

        private void AddAudiencePlayers(int mainPlayersCount)
        {
            for (int i = 0; i < mainPlayersCount; i++)
            {
                var connectionId = i + 1;
                var username = "Audience player " + i;
                this.audiencePlayersContainerUiController.ShowAudiencePlayer(connectionId, username);
            }
        }

        private bool IsAnyMainPlayerOnScreen(int mainPlayersCount)
        {
            for (int i = 0; i < mainPlayersCount; i++)
            {
                var connectionId = i + 1;
                if (this.mainPlayersContainerUiController.IsOnScreen(connectionId))
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsAnyAudiencePlayerOnScreen(int audiencePlayersCount)
        {
            for (int i = 0; i < audiencePlayersCount; i++)
            {
                var connectionId = i + 1;
                if (this.audiencePlayersContainerUiController.IsOnScreen(connectionId))
                {
                    return true;
                }
            }

            return false;
        }

        void Start()
        {
            var stateMachine = new StateMachine();
            stateMachine.SetCurrentState(this.playingState);

            var mainPlayersCount = 3;
            var audiencePlayersCount = 2;

            this.AddMainPlayers(mainPlayersCount);
            this.AddAudiencePlayers(audiencePlayersCount);

            this.CoroutineUtils.WaitForFrames(1,
                () =>
                    {
                        stateMachine.SetCurrentState(null);

                        this.CoroutineUtils.WaitForFrames(1,
                            () =>
                                {
                                    if (
                                        !this.IsAnyMainPlayerOnScreen(mainPlayersCount) &&
                                        !this.IsAnyAudiencePlayerOnScreen(audiencePlayersCount))
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