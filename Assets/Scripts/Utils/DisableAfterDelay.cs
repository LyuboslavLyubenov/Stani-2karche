using System;
using System.Collections;
using System.Linq;

using UnityEngine;

namespace Assets.Scripts.Utils
{

    using Assets.Scripts.EventArgs;

    using EventArgs = System.EventArgs;

    public class DisableAfterDelay : MonoBehaviour
    {
        public int DelayInSeconds;
        public int PassedSeconds;
        public bool DisableAfterClick = true;
        public bool UseAnimator = false;

        public event EventHandler<TimeInSecondsEventArgs> OnTimePass
        {
            add
            {
                if (this.onTimePass == null || !this.onTimePass.GetInvocationList().Contains(value))
                {
                    this.onTimePass += value;
                }
            }
            remove
            {
                this.onTimePass -= value;
            }
        }

        public event EventHandler OnTimeEnd
        {
            add
            {
                if (this.onTimeEnd == null || !this.onTimeEnd.GetInvocationList().Contains(value))
                {
                    this.onTimeEnd += value;
                }
            }
            remove
            {
                this.onTimeEnd -= value;
            }
        }

        EventHandler<TimeInSecondsEventArgs> onTimePass = delegate
            {
            };

        EventHandler onTimeEnd = delegate
            {
            };

        void OnEnable()
        {
            this.StartCoroutine(this.DisableWithDelay());
        }

        void OnDisable()
        {
            this.PassedSeconds = 0;
            this.onTimeEnd(this, EventArgs.Empty);
        }

        void FixedUpdate()
        {
            if (this.DisableAfterClick && Input.GetMouseButton(0))
            {
                this.StopCoroutine(this.DisableWithDelay());
                this.Disable();
            }
        }

        void Disable()
        {
            if (this.UseAnimator)
            {
                this.GetComponent<Animator>().SetTrigger("disable");
            }
            else
            {
                this.gameObject.SetActive(false);
            }
        }

        IEnumerator DisableWithDelay()
        {
            while (this.PassedSeconds < this.DelayInSeconds)
            {
                yield return new WaitForSeconds(1f);
                this.PassedSeconds++;
                this.onTimePass(this, new TimeInSecondsEventArgs(this.DelayInSeconds - this.PassedSeconds));
            }

            this.Disable();
        }
    }

}

