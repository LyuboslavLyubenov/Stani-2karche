using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Linq;

public class QuestionUIController : ExtendedMonoBehaviour, IQuestionUIController
{
    const int AnswersCount = 4;

    public bool ShouldPlayButtonAnimation = true;
    public bool ShowCorrectAnswerAfterError = false;

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

    public ISimpleQuestion CurrentlyLoadedQuestion
    {
        get;
        private set;
    }

    public QuestionUIController()
    {
        OnAnswerClick = delegate
        {
        };

        OnQuestionLoaded = delegate
        {
        };
    }

    void Start()
    {
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

        yield return StartCoroutine(WaitUntilAnswersAreHiddenCoroutine());

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

        CurrentlyLoadedQuestion = question;

        if (OnQuestionLoaded != null)
        {
            OnQuestionLoaded(this, new SimpleQuestionEventArgs(question));    
        }
    }

    IEnumerator WaitUntilAnswersAreHiddenCoroutine()
    {
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
    }

    void AttachButtonListener(int buttonIndex, bool isCorrect)
    {
        var button = answersButtons[buttonIndex];
        button.onClick.AddListener(() =>
            {
                DisableAnswers();

                if (ShouldPlayButtonAnimation)
                {
                    var answerText = answersTexts[buttonIndex].text;
                    ColorAnswer(answerText, true);   
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

    void ColorAnswer(string answer, bool fireClickEvent)
    {
        var correctAnswerIndex = CurrentlyLoadedQuestion.CorrectAnswerIndex;
        var answerIndex = CurrentlyLoadedQuestion.Answers.ToList().FindIndex(a => a == answer);
        var isCorrect = (correctAnswerIndex == answerIndex);
        var answerAnimator = answersAnimators[answerIndex];

        answerAnimator.SetTrigger("clicked");
        answerAnimator.SetBool("fireClickEvent", fireClickEvent);
        answerAnimator.SetBool("isCorrect", isCorrect);
    }

    void ShowCorrectAnswer()
    {
        var correctAnswerIndex = CurrentlyLoadedQuestion.CorrectAnswerIndex;
        var correctAnswer = CurrentlyLoadedQuestion.Answers[correctAnswerIndex];
        ColorAnswer(correctAnswer, false);
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
        if (ShowCorrectAnswerAfterError)
        {
            ShowCorrectAnswer();

            CoroutineUtils.WaitForSeconds(3f, () =>
                {
                    OnAnswerClick(this, new AnswerEventArgs(answer, false));
                });
            
        }
        else
        {
            OnAnswerClick(this, new AnswerEventArgs(answer, false));
        }

    }

    public void _OnCorrectAnswerAnimEnd(string answer)
    {
        OnAnswerClick(this, new AnswerEventArgs(answer, true));
    }
}