namespace Assets.Scripts.Controllers
{
    using System;

    using UnityEngine;
    using UnityEngine.UI;

    using EventArgs;

    using Network.NetworkManagers;
    using Utils.Unity;

    public class WaitingToAnswerUIController : MonoBehaviour
    {
        public GameObject RemainingSecondsObject;
        public ClientNetworkManager NetworkManager;

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

            this.UpdateTimer(this.disableAfterDelay.DelayInSeconds);

            this.disableAfterDelay.OnTimePass += this.OnTimePass;
        }

        // ReSharper disable once ArrangeTypeMemberModifiers
        void OnDisable()
        {
            this.disableAfterDelay.OnTimePass -= this.OnTimePass;
        }

        private void UpdateTimer(int remainingSeconds)
        {
            this.remainingSecondsText.text = remainingSeconds + " секунди";
        }

        private void OnTimePass(object sender, TimeInSecondsEventArgs args)
        {
            this.UpdateTimer(args.Seconds);
        }
    }

}
