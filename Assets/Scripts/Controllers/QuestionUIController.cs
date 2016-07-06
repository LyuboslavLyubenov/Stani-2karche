using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Linq;

public class QuestionUIController : MonoBehaviour
{
    const int AnswersCount = 4;
   
    public EventHandler<AnswerEventArgs> OnAnswerClick = delegate
    {
    };

    public EventHandler<QuestionEventArgs> OnQuestionLoaded = delegate
    {
    };

    public bool ShouldPlayButtonAnimation = true;

    Text questionText = null;
    Text[] answersTexts = new Text[AnswersCount];
    Button[] answersButtons = new Button[AnswersCount];
    Animator[] answersAnimators = new Animator[AnswersCount];
    HiddenAnswerAnimatorController[] hiddenAnswerAnimatorControllers = new HiddenAnswerAnimatorController[AnswersCount];

    void Start()
    {
        StartCoroutine(InitializeCoroutine());
    }

    IEnumerator InitializeCoroutine()
    {
        var answers = GameObject.FindGameObjectsWithTag("Answer").Take(AnswersCount)
            .ToArray();

        yield return null;

        questionText = GameObject.FindWithTag("QuestionText").GetComponent<Text>();

        yield return null;

        for (int i = 0; i < answers.Length; i++)
        {
            answersButtons[i] = answers[i].GetComponent<Button>();
            answersTexts[i] = answers[i].transform.GetComponentInChildren<Text>();
            answersAnimators[i] = answers[i].GetComponent<Animator>();
            hiddenAnswerAnimatorControllers[i] = answersAnimators[i].GetBehaviour<HiddenAnswerAnimatorController>();
        
            yield return null;
        }
    }

    IEnumerator LoadQuestionCoroutine(Question question)
    {
        yield return new WaitForEndOfFrame();

        questionText.text = question.Text;

        DisableAnswers();
        HideAllAnswers();
            
        for (int i = 0; i < AnswersCount; i++)
        {
            var hiddenAnswerAnimatorController = hiddenAnswerAnimatorControllers[i];

            while (true)
            {
                var finished = hiddenAnswerAnimatorController.Hidden; 

                if (finished)
                {
                    break;
                }

                yield return new WaitForEndOfFrame();
            }
        }

        for (int i = 0; i < AnswersCount; i++)
        {
            var buttonIndex = i;
            var isCorrect = (buttonIndex == question.CorrectAnswerIndex);
            var answerObj = answersButtons[buttonIndex].gameObject;

            answerObj.SetActive(true);

            answersTexts[buttonIndex].text = question.Answers[buttonIndex];
            answersButtons[buttonIndex].interactable = true;
            answersButtons[buttonIndex].onClick.RemoveAllListeners();

            AttachButtonListener(buttonIndex, isCorrect);

            yield return new WaitForEndOfFrame();
        }

        if (OnQuestionLoaded != null)
        {
            OnQuestionLoaded(this, new QuestionEventArgs(question));    
        }
    }

    void AttachButtonListener(int buttonIndex, bool isCorrect)
    {
        var button = answersButtons[buttonIndex];
        button.onClick.AddListener(() =>
            {
                DisableAnswers();

                if (ShouldPlayButtonAnimation)
                {
                    PlayClickedAnimation(buttonIndex, isCorrect);    
                }
                else
                {
                    var answer = answersTexts[buttonIndex].text;
                    OnAnswerClick(this, new AnswerEventArgs(answer, isCorrect));
                }
            });
    }

    void DisableAnswers()
    {
        for (int i = 0; i < answersButtons.Length; i++)
        {
            answersButtons[i].interactable = false;
        }
    }

    void PlayClickedAnimation(int buttonIndex, bool isCorrect)
    {
        answersAnimators[buttonIndex].SetTrigger("clicked");
        answersAnimators[buttonIndex].SetBool("isCorrect", isCorrect);
    }

    public void DeactivateAnswerObj(int index)
    {
        if (index < 0 || index >= AnswersCount)
        {
            throw new ArgumentOutOfRangeException("index");
        }

        answersAnimators[index].SetBool("hidden", true);
    }

    public void HideAllAnswers()
    {
        for (int i = 0; i < AnswersCount; i++)
        {
            hiddenAnswerAnimatorControllers[i].Hidden = false;

            if (answersAnimators[i].gameObject.activeInHierarchy)
            {
                answersAnimators[i].SetTrigger("hide");    
            }
        }
    }

    public void LoadQuestion(Question question)
    {
        StartCoroutine(LoadQuestionCoroutine(question));
    }

    public void _OnIncorrectAnswerAnimEnd(string answer)
    {
        OnAnswerClick(this, new AnswerEventArgs(answer, false));
    }

    public void _OnCorrectAnswerAnimEnd(string answer)
    {
        OnAnswerClick(this, new AnswerEventArgs(answer, true));
    }
}