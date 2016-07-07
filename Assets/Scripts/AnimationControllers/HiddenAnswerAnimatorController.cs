using UnityEngine;
using System.Collections;

public class HiddenAnswerAnimatorController : StateMachineBehaviour
{
    public bool Hidden = false;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Hidden = false;
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Hidden = true;
    }

}
