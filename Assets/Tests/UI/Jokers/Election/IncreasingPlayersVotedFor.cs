using UnityEngine;

namespace Tests.UI.Jokers.Election
{

    using Assets.Scripts.Interfaces.Controllers;

    using Controllers.EveryBodyVsTheTeacher.Jokers.Election;

    using UnityEngine.UI;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Zenject.Source.Usage;

    public class IncreasingPlayersVotedFor : MonoBehaviour
    {
        [Inject]
        private IJokerElectionUIController jokerElectionUiController;

        [SerializeField]
        private Text votedForCountText;

        // Use this for initialization
        void Start()
        {
            this.jokerElectionUiController.AddThumbsUp();

            if (this.votedForCountText.text == "1")
            {
                IntegrationTest.Pass();
            }
            else
            {
                IntegrationTest.Fail();
            }

            this.jokerElectionUiController.AddThumbsUp();

            if (this.votedForCountText.text == "2")
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
