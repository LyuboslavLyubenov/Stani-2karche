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

    public void SetMark(string mark)
    {
        if (string.IsNullOrEmpty(mark))
        {
            throw new ArgumentNullException("mark", "Mark cannot be null");
        }

        MarkText.text = mark;
        animator.SetTrigger("MarkIncreased");
    }
}
