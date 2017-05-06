using DisableAfterDelay = Utils.Unity.DisableAfterDelay;

namespace Scripts.Controllers.EveryBodyVsTheTeacher
{

    using System;

    using Assets.Scripts.Interfaces.Controllers;

    using Extensions;

    using UnityEngine;
    using UnityEngine.UI;

    public class ChangedRoundUIController : DisableAfterDelay, IChangedRoundUIController
    {
        private const int IntervalInSeconds = 10;

        [SerializeField]
        private Text RoundsNumberText;
        
        public int Round
        {
            get
            {
                return this.RoundsNumberText.text.ConvertTo<int>();
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException();
                }

                this.RoundsNumberText.text = value.ToString();
            }
        }
        
        protected override void Initialize()
        {
            base.UseAnimator = true;
            base.InvervalInSeconds = IntervalInSeconds;
        }
    }
}