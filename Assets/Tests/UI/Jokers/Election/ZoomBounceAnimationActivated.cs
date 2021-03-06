﻿namespace Tests.UI.Jokers.Election
{

    using Assets.Scripts.Interfaces.Controllers;

    using UnityEngine;
    using UnityTestTools.IntegrationTestsFramework.TestRunner;
    using Utils.Unity;

    using Zenject;

    public class ZoomBounceAnimationActivated : ExtendedMonoBehaviour
    {
        [Inject]
        private IJokerElectionUIController jokerElectionUiController;

        [SerializeField]
        private Animator animator;

        [SerializeField]
        private VoteType voteType;

        void Start()
        {
            if (this.voteType == VoteType.For)
            {
                this.jokerElectionUiController.AddThumbsUp();
            }
            else
            {
                this.jokerElectionUiController.AddThumbsDown();
            }
            this.CoroutineUtils.RepeatEveryNthFrame(5,
                () =>
                    {
                        if (this.animator.GetCurrentAnimatorStateInfo(0).IsName("ZoomBounce"))
                        {
                            IntegrationTest.Pass();
                        }
                    });
        }
    }

    public enum VoteType
    {
        For,
        Against
    }
}