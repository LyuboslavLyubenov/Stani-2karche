namespace Tests.UI.Jokers.Election
{

    using Assets.Scripts.Interfaces.Controllers;

    using UnityEngine;
    using UnityEngine.UI;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Zenject;

    public class IncreasingPlatersVotedAgainst : MonoBehaviour
    {
        [Inject]
        private IJokerElectionUIController jokerElectionUiController;

        [SerializeField]
        private Text votedAgainsCounText;
        
        void Start()
        {
            this.jokerElectionUiController.AddThumbsDown();

            if (this.votedAgainsCounText.text == "1")
            {
                IntegrationTest.Pass();
            }
            else
            {
                IntegrationTest.Fail();
            }

            this.jokerElectionUiController.AddThumbsDown();

            if (this.votedAgainsCounText.text == "2")
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