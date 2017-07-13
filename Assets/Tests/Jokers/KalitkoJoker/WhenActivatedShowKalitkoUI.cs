namespace Assets.Tests.Jokers.KalitkoJoker
{
    using Assets.Scripts.Interfaces;

    using UnityEngine;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Zenject;

    public class WhenActivatedShowKalitkoUI : MonoBehaviour
    {
        [Inject]
        private GameObject kalitkoJokerUI;

        [Inject]
        private IJoker kalitkoJoker;

        void Start()
        {
            this.kalitkoJoker.Activate();

            if (this.kalitkoJokerUI.activeSelf)
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
