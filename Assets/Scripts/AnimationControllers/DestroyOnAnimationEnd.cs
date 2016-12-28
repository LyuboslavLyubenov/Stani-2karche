namespace Assets.Scripts.AnimationControllers
{

    using UnityEngine;

    public class DestroyOnAnimationEnd : StateMachineBehaviour
    {
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Destroy(animator.gameObject);
        }
    }

}
