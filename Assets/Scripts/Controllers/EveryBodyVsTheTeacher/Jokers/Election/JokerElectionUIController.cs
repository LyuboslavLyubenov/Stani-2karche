﻿namespace Controllers.EveryBodyVsTheTeacher.Jokers.Election
{
    using System;

    using Assets.Scripts.Extensions;
    using Assets.Scripts.Interfaces.Controllers;

    using Extensions;

    using UnityEngine;
    using UnityEngine.UI;

    public class JokerElectionUIController : MonoBehaviour, IJokerElectionUIController
    {
        public event EventHandler OnVotedFor = delegate {};
        public event EventHandler OnVotedAgainst = delegate {};

        [SerializeField]
        private Image jokerImageObj;

        [SerializeField]
        private Animator thumbsUpAnimator;

        [SerializeField]
        private Text thumbsUpCountText;

        [SerializeField]
        private Animator thumbsDownAnimator;

        [SerializeField]
        private Text thumbsDownCountText;

        void OnDisable()
        {
            this.jokerImageObj.sprite = null;
            this.thumbsUpCountText.text = "0";
            this.thumbsDownCountText.text = "0";
        }

        public void SetJokerImage(Sprite jokerImage)
        {
            if (jokerImage == null)
            {
                throw new ArgumentNullException("jokerImage");
            }

            this.jokerImageObj.sprite = jokerImage;
        }

        public void AddThumbsUp()
        {
            var peopleVotedFor = this.thumbsUpCountText.text.ConvertTo<int>();
            this.thumbsUpCountText.text = (++peopleVotedFor).ToString();

            this.thumbsUpAnimator.SetTrigger("zoomBounce");

            this.OnVotedFor(this, EventArgs.Empty);
        }

        public void AddThumbsDown()
        {
            var peopleAgainst = this.thumbsDownCountText.text.ConvertTo<int>();
            this.thumbsDownCountText.text = (++peopleAgainst).ToString();

            this.thumbsDownAnimator.SetTrigger("zoomBounce");

            this.OnVotedAgainst(this, EventArgs.Empty);
        }
    }
}