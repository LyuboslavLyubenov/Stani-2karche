using UnityEngine;
using System.Collections;

public class RightAnswerAnimationController : StateMachineBehaviour
{
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var playingUI = GameObject.FindWithTag("PlayingUI");
        playingUI.GetComponent<PlayingUIController>().OnCorrectAnswerClick();
    }
}
