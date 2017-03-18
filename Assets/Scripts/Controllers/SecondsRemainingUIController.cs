namespace Controllers
{

    using System;

    using UnityEngine.UI;

    using Utils.Unity;

    public class SecondsRemainingUIController : ExtendedMonoBehaviour
    {
        public Text SecondsText;

        public int RemainingSecondsToAnswer
        {
            get;
            private set;
        }

        public bool Paused
        {
            get;
            set;
        }

        // ReSharper disable once ArrangeTypeMemberModifiers
        void Start()
        {
            this.Paused = false;

            this.CoroutineUtils.RepeatEverySeconds(1, () =>
                {
                    if (this.RemainingSecondsToAnswer > 0 && !this.Paused)
                    {
                        this.RemainingSecondsToAnswer--;
                    }

                    this.SecondsText.text = this.RemainingSecondsToAnswer.ToString();
                });
        }

        public void SetSeconds(int seconds)
        {
            if (seconds <= 0)
            {
                throw new ArgumentOutOfRangeException("seconds");
            }

            this.Paused = false;

            this.RemainingSecondsToAnswer = seconds;
            this.SecondsText.text = this.RemainingSecondsToAnswer.ToString();
        }
    }

}
