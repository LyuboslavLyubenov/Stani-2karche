using UnityEngine;
using UnityEngine.UI;

public class WrongAnswerAnimationController : StateMachineBehaviour
{
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var questionPanel = GameObject.FindWithTag("QuestionPanel");
        var answerText = animator.gameObject.GetComponentInChildren<Text>().text;
        questionPanel.GetComponent<QuestionUIController>()._OnIncorrectAnswerAnimEnd(answerText);
    }
}
