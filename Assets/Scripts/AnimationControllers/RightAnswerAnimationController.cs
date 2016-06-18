using UnityEngine;
using UnityEngine.UI;

public class RightAnswerAnimationController : StateMachineBehaviour
{
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var questionPanel = GameObject.FindWithTag("QuestionPanel");
        var answerText = animator.gameObject.GetComponentInChildren<Text>().text;
        var questionUIController = questionPanel.GetComponent<QuestionUIController>();
        questionUIController._OnCorrectAnswerAnimEnd(answerText);
    }
}
