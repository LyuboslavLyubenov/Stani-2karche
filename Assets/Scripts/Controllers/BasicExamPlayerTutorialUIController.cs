using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BasicExamPlayerTutorialUIController : MonoBehaviour
{
    const string AlreadyPlayedKey = "TutorialShowed - BasicExamPlayerTutorial";

    public bool RepeatTutorial = false;
    public bool AllowSkipping = true;

    bool activated = false;

    Animator animator;

    public void Activate()
    {
        if (PlayerPrefs.HasKey(AlreadyPlayedKey) && !RepeatTutorial)
        {
            return;
        }

        animator = GetComponent<Animator>();

        GetComponent<Image>().enabled = true;
        animator.enabled = true; 

        PlayerPrefs.SetInt(AlreadyPlayedKey, 1);

        activated = true;

        StartCoroutine(_UpdateCoroutine());
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }

    IEnumerator _UpdateCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);

            var mouseClicked = Input.GetMouseButton(0);

            if (activated && AllowSkipping && mouseClicked)
            {
                var nextStateHash = animator.GetNextAnimatorStateInfo(0).shortNameHash;
                animator.CrossFade(nextStateHash, 0.5f);
            }    
        }
    }
}
