namespace Controllers
{

    using System;
    using System.Timers;

    using UnityEngine.UI;

    using Utils.Unity;

    public class SecondsRemainingUIController : UnityTimer
    {
        public Text SecondsText;

        public int RemainingSecondsToAnswer
        {
            get;
            private set;
        }

        private void OnSecondPassed(object sender, EventArgs eventArgs)
        {
            this.RemainingSecondsToAnswer--;
            this.SecondsText.text = this.RemainingSecondsToAnswer.ToString();
        }
        
        protected override void Initiaze()
        {
            base.OnSecondPassed += OnSecondPassed;
        }
        
        public override void StartTimer()
        {            
            base.StartTimer();

            this.RemainingSecondsToAnswer = this.InvervalInSeconds;
            this.SecondsText.text = this.RemainingSecondsToAnswer.ToString();
        }
    }

}