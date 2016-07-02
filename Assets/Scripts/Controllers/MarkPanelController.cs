using UnityEngine;
using System.Collections;

public class MarkPanelController : MonoBehaviour
{

    public GameData gameData;

    Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        gameData.MarkIncrease += OnMarkIncrease;
    }

    void OnMarkIncrease(object sender, System.EventArgs args)
    {
        animator.SetTrigger("MarkIncreased");
    }
}
