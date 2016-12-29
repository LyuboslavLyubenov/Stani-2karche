namespace Assets.Scripts.Controllers
{
    using System;

    using UnityEngine;
    using UnityEngine.UI;

    using Utils.Unity;

    public class MarkPanelController : ExtendedMonoBehaviour
    {
        public Text MarkText;

        private Animator animator;

        public string Mark
        {
            get
            {
                return this.MarkText.text;
            }
        }

        // ReSharper disable once UnusedMember.Local
        void Start()
        {
            this.animator = this.GetComponent<Animator>();
            this.MarkText.GetComponentInChildren<Text>();

            if (this.animator == null)
            {
                throw new Exception("Animator not found on Object holding MarkPanelController");
            }
        }

        public void SetMark(string mark)
        {
            if (string.IsNullOrEmpty(mark))
            {
                throw new ArgumentNullException("mark", "Mark cannot be null");
            }

            this.MarkText.text = mark;
            this.animator.SetTrigger("MarkIncreased");
        }
    }

}
