namespace AnimationControllers
{
    using UnityEngine;

    public class DisableAfterAnimationOver : StateMachineBehaviour
    {
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.gameObject.SetActive(false);
        }
    }
}