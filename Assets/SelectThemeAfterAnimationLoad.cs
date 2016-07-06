﻿using UnityEngine;
using System.Collections;

public class SelectThemeAfterAnimationLoad : StateMachineBehaviour
{
    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.gameObject.GetComponent<ChooseThemeUIController>().Initialize();
    }

}
