namespace Controllers
{
    using System;

    using Assets.Scripts.Interfaces.Controllers;

    using UnityEngine.UI;

    public class SecondsRemainingUIController : UnityTimer, ISecondsRemainingUIController
    {
        public Text SecondsText;

        public int RemainingSecondsToAnswer
        {
            get;
            private set;
        }

        private void _OnSecondPassed(object sender, EventArgs eventArgs)
        {
            this.RemainingSecondsToAnswer--;
            this.SecondsText.text = this.RemainingSecondsToAnswer.ToString();
        }
        
        protected override void Initiaze()
        {
            base.OnSecondPassed += _OnSecondPassed;
        }
        
        public override void StartTimer()
        {            
            base.StartTimer();

            this.RemainingSecondsToAnswer = this.InvervalInSeconds;
            this.SecondsText.text = this.RemainingSecondsToAnswer.ToString();
        }
    }
}