namespace Utils.Unity
{

    using System;

    using Controllers;

    using UnityEngine;

    public class DisableAfterDelay : UnityTimer
    {
        private const int DefaultDelayInSeconds = 8;

        public bool DisableAfterClick = true;
        public bool UseAnimator = false;

        private bool deactivating = false;

        protected override void Initialize()
        {
            if (base.InvervalInSeconds == 0)
            {
                base.InvervalInSeconds = DefaultDelayInSeconds;
            }

            this.OnFinished += this.OnTimeOver;
            base.Initialize();
        }

        private void OnTimeOver(object sender, EventArgs args)
        {
            this.Disable();
        }

        // ReSharper disable once ArrangeTypeMemberModifiers
        void OnEnable()
        {
            this.deactivating = false;
            this.StartTimer();
        }

        // ReSharper disable once ArrangeTypeMemberModifiers
        void OnDisable()
        {
            if (this.Running)
            {
                this.StopTimer();
            }
        }

        // ReSharper disable once ArrangeTypeMemberModifiers
        void FixedUpdate()
        {
            if (this.DisableAfterClick && Input.GetMouseButton(0) && !this.deactivating)
            {
                this.StopTimer();
                this.Disable();
            }
        }

        private void Disable()
        {
            this.deactivating = true;

            if (this.UseAnimator)
            {
                this.GetComponent<Animator>().SetTrigger("disable");
            }
            else
            {
                this.gameObject.SetActive(false);
            }
        }
    }
}