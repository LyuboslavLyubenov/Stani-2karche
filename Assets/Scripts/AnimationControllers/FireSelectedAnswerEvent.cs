namespace AnimationControllers
{

    using Controllers;

    using UnityEngine;
    using UnityEngine.UI;

    public class FireSelectedAnswerEvent : StateMachineBehaviour
    {
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var questionPanel = GameObject.FindWithTag("QuestionPanel");
            var questionUIController = questionPanel.GetComponent<QuestionUIController>();
            var shouldFireClickEvent = animator.GetBool("fireClickEvent");

            if (shouldFireClickEvent)
            {
                var answer = animator.gameObject.GetComponentInChildren<Text>().text;
                var isCorrect = animator.GetBool("isCorrect");

                if (isCorrect)
                {
                    questionUIController._OnCorrectAnswerAnimEnd(answer);    
                }
                else
                {
                    questionUIController._OnIncorrectAnswerAnimEnd(answer);
                }
            }
        }
    }

}
