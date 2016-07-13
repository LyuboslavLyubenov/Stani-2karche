using UnityEngine;
using System.Collections;
using System;

public class MarkPanelController : MonoBehaviour
{
    public GameData GameData;

    Animator animator;

    void Start()
    {
        if (GameData == null)
        {
            throw new Exception("Gamedata is null on MarkPanelController");
        }

        animator = GetComponent<Animator>();

        if (animator == null)
        {
            throw new Exception("Animator not found on Object holding MarkPanelController");
        }

        GameData.MarkIncrease += OnMarkIncrease;
    }

    void OnMarkIncrease(object sender, System.EventArgs args)
    {
        animator.SetTrigger("MarkIncreased");
    }
}
