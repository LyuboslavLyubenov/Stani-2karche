namespace Utils.Unity
{
    using Controllers;

    using UnityEngine;

    public class DisableAfterDelay : UnityTimer
    {
        public bool DisableAfterClick = true;
        public bool UseAnimator = false;
        
        // ReSharper disable once ArrangeTypeMemberModifiers
        void OnEnable()
        {
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
            if (this.DisableAfterClick && Input.GetMouseButton(0))
            {
                this.StopTimer();
                this.Disable();
            }
        }

        private void Disable()
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
    }

}

