using System;

using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Controllers
{

    using Assets.Scripts.Utils;
    using Assets.Scripts.Utils.Unity;

    public class MarkPanelController : ExtendedMonoBehaviour
    {
        public Text MarkText;

        Animator animator;

        public string Mark
        {
            get
            {
                return this.MarkText.text;
            }
        }

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
