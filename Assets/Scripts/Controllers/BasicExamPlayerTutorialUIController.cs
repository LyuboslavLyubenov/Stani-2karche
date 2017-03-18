namespace Controllers
{

    using System.Collections;

    using UnityEngine;
    using UnityEngine.UI;

    public class BasicExamPlayerTutorialUIController : MonoBehaviour
    {
        private const string AlreadyPlayedKey = "TutorialShowed - BasicExamPlayerTutorial";

        public bool RepeatTutorial = false;
        public bool AllowSkipping = true;

        private bool activated = false;

        private Animator animator;

        public void Activate()
        {
            if (PlayerPrefs.HasKey(AlreadyPlayedKey) && !this.RepeatTutorial)
            {
                return;
            }

            this.animator = this.GetComponent<Animator>();

            this.GetComponent<Image>().enabled = true;
            this.animator.enabled = true; 

            PlayerPrefs.SetInt(AlreadyPlayedKey, 1);

            this.activated = true;

            this.StartCoroutine(this._UpdateCoroutine());
        }

        // ReSharper disable once ArrangeTypeMemberModifiers
        void OnDisable()
        {
            this.StopAllCoroutines();
        }

        private IEnumerator _UpdateCoroutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.1f);

                var mouseClicked = Input.GetMouseButton(0);

                if (this.activated && this.AllowSkipping && mouseClicked)
                {
                    var nextStateHash = this.animator.GetNextAnimatorStateInfo(0).shortNameHash;
                    this.animator.CrossFade(nextStateHash, 0.5f);
                }    
            }
        }
    }

}
