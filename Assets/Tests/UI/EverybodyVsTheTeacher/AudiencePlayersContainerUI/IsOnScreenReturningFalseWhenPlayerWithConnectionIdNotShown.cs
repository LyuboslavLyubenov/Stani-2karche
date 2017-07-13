namespace Assets.Tests.UI.EverybodyVsTheTeacher.AudiencePlayersContainerUI
{
    using Interfaces.Controllers;

    using UnityEngine;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Zenject;

    public class IsOnScreenReturningFalseWhenPlayerWithConnectionIdNotShown : MonoBehaviour
    {
        [Inject]
        private IAudiencePlayersContainerUIController audiencePlayersContainerUIController;
        
        void Start()
        {
            if (!this.audiencePlayersContainerUIController.IsOnScreen(10))
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