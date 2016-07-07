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

    void Start()
    {
        StartCoroutine(InitializeCoroutine());
    }

    IEnumerator InitializeCoroutine()
    {
        var answers = GameObject.FindGameObjectsWithTag("Answer").Take(AnswersCount)
            .ToArray();

        questionText = GameObject.FindWithTag("QuestionText").GetComponent<Text>();

        yield return null;

        for (int i = 0; i < answers.Length; i++)
        {
            answersButtons[i] = answers[i].GetComponent<Button>();
            answersTexts[i] = answers[i].transform.GetComponentInChildren<Text>();
            answersAnimators[i] = answers[i].GetComponent<Animator>();
 
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
            while (true)
            {
                var stateInfo = answersAnimators[i].GetCurrentAnimatorStateInfo(0);

                if (stateInfo.IsTag("Hidden"))
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

            answersTexts[buttonIndex].text = question.Answers[buttonIndex];
            answersButtons[buttonIndex].interactable = true;
            answersButtons[buttonIndex].onClick.RemoveAllListeners();

            AttachButtonListener(buttonIndex, isCorrect);
        }

        ShowAllAnswers();

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

    void ShowAnswer(int index)
    {
        if (index < 0 || index >= AnswersCount)
        {
            throw new ArgumentOutOfRangeException("index");
        }

        var currentState = answersAnimators[index].GetCurrentAnimatorStateInfo(0);

        if (!currentState.IsTag("Visible"))
        {
            answersAnimators[index].SetTrigger("show");
        }
    }

    public void HideAnswer(int index)
    {
        if (index < 0 || index >= AnswersCount)
        {
            throw new ArgumentOutOfRangeException("index");
        }

        var currentState = answersAnimators[index].GetCurrentAnimatorStateInfo(0);

        if (!currentState.IsTag("Hidden"))
        {
            answersAnimators[index].SetTrigger("hide");    
        }
    }

    public void HideAllAnswers()
    {
        for (int i = 0; i < AnswersCount; i++)
        {
            HideAnswer(i);
        }
    }

    public void ShowAllAnswers()
    {
        for (int i = 0; i < AnswersCount; i++)
        {
            ShowAnswer(i);  
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