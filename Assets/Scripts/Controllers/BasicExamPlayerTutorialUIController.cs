using System.Collections;

using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Controllers
{

    public class BasicExamPlayerTutorialUIController : MonoBehaviour
    {
        const string AlreadyPlayedKey = "TutorialShowed - BasicExamPlayerTutorial";

        public bool RepeatTutorial = false;
        public bool AllowSkipping = true;

        bool activated = false;

        Animator animator;

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

        void OnDisable()
        {
            this.StopAllCoroutines();
        }

        IEnumerator _UpdateCoroutine()
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
