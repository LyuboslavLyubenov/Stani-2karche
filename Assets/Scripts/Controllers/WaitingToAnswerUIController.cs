using System;

using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Controllers
{

    using Assets.Scripts.EventArgs;
    using Assets.Scripts.Network;
    using Assets.Scripts.Utils;

    public class WaitingToAnswerUIController : MonoBehaviour
    {
        public GameObject RemainingSecondsObject;
        public ClientNetworkManager NetworkManager;

        Text remainingSecondsText;
        DisableAfterDelay disableAfterDelay = null;

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

        void OnDisable()
        {
            this.disableAfterDelay.OnTimePass -= this.OnTimePass;
        }

        void UpdateTimer(int remainingSeconds)
        {
            this.remainingSecondsText.text = remainingSeconds + " секунди";
        }

        void OnTimePass(object sender, TimeInSecondsEventArgs args)
        {
            this.UpdateTimer(args.Seconds);
        }
    }

}
