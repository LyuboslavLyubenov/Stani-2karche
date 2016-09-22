using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Linq;

public class QuestionUIController : MonoBehaviour, IQuestionUIController
{
    const int AnswersCount = 4;

    public bool ShouldPlayButtonAnimation = true;

    bool initialized = false;

    Text questionText = null;
    Text[] answersTexts = new Text[AnswersCount];
    Button[] answersButtons = new Button[AnswersCount];
    Animator[] answersAnimators = new Animator[AnswersCount];

    public EventHandler<AnswerEventArgs> OnAnswerClick
    {
        get;
        set;
    }

    public EventHandler<SimpleQuestionEventArgs> OnQuestionLoaded
    {
        get;
        set;
    }

    void Start()
    {
        OnAnswerClick = delegate
        {
        };

        OnQuestionLoaded = delegate
        {
        };
        
        StartCoroutine(InitializeCoroutine());
    }

    IEnumerator InitializeCoroutine()
    {
        yield return null;

        var answers = GameObject.FindGameObjectsWithTag("Answer").Take(AnswersCount)
            .ToArray();

        if (answers.Length != 4)
        {
            throw new NullReferenceException("Invalid answers count. Found " + answers.Length + " expected 4.");
        }

        questionText = GameObject.FindWithTag("QuestionText").GetComponent<Text>();

        if (questionText == null)
        {
            throw new NullReferenceException("Cannot found Text component on questionText");
        }

        yield return null;

        for (int i = 0; i < answers.Length; i++)
        {
            var answerButton = answers[i].GetComponent<Button>();
            var answerText = answers[i].transform.GetComponentInChildren<Text>();
            var answerAnimator = answers[i].GetComponent<Animator>();

            if (answerButton == null)
            {
                throw new Exception("Answer must be button");
            }

            if (answerText == null)
            {
                throw new Exception("Answer must have text component");
            }

            if (answerAnimator == null)
            {
                throw new Exception("Answer must have animator component");
            }

            answersButtons[i] = answerButton;
            answersTexts[i] = answerText;
            answersAnimators[i] = answerAnimator;
 
            yield return null;
        }

        initialized = true;
    }

    IEnumerator LoadQuestionCoroutine(ISimpleQuestion question)
    {
        yield return new WaitUntil(() => initialized);
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
            OnQuestionLoaded(this, new SimpleQuestionEventArgs(question));    
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

    public void LoadQuestion(ISimpleQuestion question)
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