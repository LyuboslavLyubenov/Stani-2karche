namespace Assets.Tests.Jokers.ConsultWithTeacherJoker
{

    using Assets.Scripts.Interfaces;

    using Interfaces;
    using Interfaces.Network.NetworkManager;

    using UnityEngine;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Zenject.Source.Usage;

    public class WhenActivatedShowLoadingScreenUntilReceivedSettings : MonoBehaviour
    {
        [Inject]
        private IClientNetworkManager networkManager;
        
        [Inject]
        private ISimpleQuestion question;
        
        [Inject]
        private GameObject loadingUI;

        [Inject]
        private IJoker joker;

        void Start()
        {
            this.joker.Activate();

            if (this.loadingUI.activeSelf)
            {
                IntegrationTest.Pass();
            }

        }
    }

}