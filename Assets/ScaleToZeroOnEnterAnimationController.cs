using UnityEngine;
using System.Collections;

public class ScaleToZeroOnEnterAnimationController : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.transform.localScale = new Vector3(0, 0, 0);
    }
}
