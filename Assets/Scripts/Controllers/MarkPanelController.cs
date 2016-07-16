using UnityEngine;
using System;
using UnityEngine.UI;

public class MarkPanelController : ExtendedMonoBehaviour
{
    public Text MarkText;

    Animator animator;

    public string Mark
    {
        get
        {
            return MarkText.text;
        }
        set
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException("value", "Mark cannot be null");
            }

            SetMark(value);
        }
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        MarkText.GetComponentInChildren<Text>();

        if (animator == null)
        {
            throw new Exception("Animator not found on Object holding MarkPanelController");
        }
    }

    void SetMark(string mark)
    {
        MarkText.text = mark;
        animator.SetTrigger("MarkIncreased");
    }
}
