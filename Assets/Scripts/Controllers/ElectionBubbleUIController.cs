namespace Controllers
{

    using System;

    using Assets.Scripts.Extensions;

    using Extensions;

    using Interfaces.Controllers;

    using UnityEngine;
    using UnityEngine.UI;

    public class ElectionBubbleUIController : MonoBehaviour, IElectionBubbleUIController
    {
        private Text voteCountText;

        public int VoteCount
        {
            get
            {
                return this.voteCountText.text.ConvertTo<int>();
            }
            private set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException();
                }

                this.voteCountText.text = value.ToString();
            }
        }

        void Awake()
        {
            this.voteCountText = this.GetComponentInChildren<Text>();
        }

        public void AddVote()
        {
            this.VoteCount++;
        }

        public void ResetVotesToZero()
        {
            this.voteCountText.text = "0";
        }
    }
}