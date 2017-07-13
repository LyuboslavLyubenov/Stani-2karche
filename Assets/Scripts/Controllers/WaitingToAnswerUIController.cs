namespace Controllers
{

    using System;

    using Assets.Scripts.Interfaces.Utils;

    using UnityEngine;
    using UnityEngine.UI;

    using Utils.Unity;

    public class WaitingToAnswerUIController : MonoBehaviour
    {
        public GameObject RemainingSecondsObject;

        private Text remainingSecondsText;

        private DisableAfterDelay disableAfterDelay = null;

        // ReSharper disable once ArrangeTypeMemberModifiers
        void OnEnable()
        {
            if (this.RemainingSecondsObject == null)
            {
                throw new NullReferenceException("RemainingSecondsObjects is null on WaitingToAnswerUIController obj");
            }

            this.disableAfterDelay = this.GetComponent<DisableAfterDelay>();

            if (this.disableAfterDelay == null)
            {
                throw new Exception("WaitingToAnswerUIController obj must have DisableAfterDelay component");
            }

            this.remainingSecondsText = this.RemainingSecondsObject.GetComponent<Text>();

            if (this.remainingSecondsText == null)
            {
                throw new Exception("RemainingSecondsObject obj is null or doesnt have Text component");
            }

            this.UpdateTimer(this.disableAfterDelay.InvervalInSeconds);

            this.disableAfterDelay.OnSecondPassed += this.OnTimePass;
        }

        // ReSharper disable once ArrangeTypeMemberModifiers
        void OnDisable()
        {
            this.disableAfterDelay.OnSecondPassed -= this.OnTimePass;
        }

        private void UpdateTimer(int remainingSeconds)
        {
            this.remainingSecondsText.text = remainingSeconds + " секунди";
        }

        private void OnTimePass(object sender, EventArgs args)
        {
            this.UpdateTimer(((IUnityTimer)sender).InvervalInSeconds);
        }
    }
}
