using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class MarkPanelController : MonoBehaviour
{
    Animator animator;
    Text markText;

    public string Mark
    {
        get
        {
            return markText.text;
        }
        set
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException("value", "Mark cannot be null");
            }

            markText.text = value;
            animator.SetTrigger("MarkIncreased");
        }
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        markText.GetComponentInChildren<Text>();

        if (animator == null)
        {
            throw new Exception("Animator not found on Object holding MarkPanelController");
        }
    }
}
