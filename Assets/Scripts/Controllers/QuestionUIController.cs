using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections.Generic;

public class QuestionUIController : ExtendedMonoBehaviour, IQuestionUIController
{
    const int DistanceBetweenAnswerButton = 10;

    public int AnswersCount = 4;
    public bool ShouldPlayButtonAnimation = true;
    public bool ShowCorrectAnswerAfterError = false;

    bool initialized = false;

    Text questionText = null;
    Text[] answersTexts = null;
    Button[] answersButtons = null;
    Animator[] answersAnimators = null;

    Transform answersPanel = null;
    RectTransform leftColumn = null;
    RectTransform rightColumn = null;

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

    void InitializeAnswers(int answersCount)
    {
        var answers = GenerateAnswers(answersCount);
        LoadAnswersComponents(answers);
        this.AnswersCount = answersCount;
    }

    void LoadAnswersComponents(GameObject[] answers)
    {
        answersTexts = new Text[answers.Length];
        answersButtons = new Button[answers.Length];
        answersAnimators = new Animator[answers.Length];

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
        }
    }

    GameObject[] GenerateAnswers(int count)
    {
        var answerPrefab = Resources.Load<GameObject>("Prefabs/Answer");
        var answers = new GameObject[count];

        for (int i = 0; i < answers.Length; i++)
        {
            var parent = (i % 2 == 0) ? leftColumn : rightColumn;
            var parentRectTransform = (i % 2 == 0) ? leftColumn : rightColumn;
            var parentHeight = parentRectTransform.sizeDelta.y;
            var answerObjsInColumn = (int)Math.Ceiling(count / 2d);
            var distanceBetweenAnswersInColumnSum = (DistanceBetweenAnswerButton / 2 * answerObjsInColumn);
            var sizeY = ((parentHeight - distanceBetweenAnswersInColumnSum) / answerObjsInColumn);
            var y = (sizeY / 2) + (parent.childCount * (DistanceBetweenAnswerButton + (int)sizeY));
            var answer = (GameObject)Instantiate(answerPrefab, parent, false);
            var answerRectTransform = answer.GetComponent<RectTransform>();

            answerRectTransform.sizeDelta = new Vector2(answerRectTransform.sizeDelta.x, (float)sizeY);
            answerRectTransform.anchoredPosition = new Vector2(answerRectTransform.anchoredPosition.x, (float)-y);
            answers[i] = answer;
        }

        return answers;
    }

    IEnumerator InitializeCoroutine()
    {
        yield return null;

        questionText = GameObject.FindWithTag("QuestionText").GetComponent<Text>();
        answersPanel = transform.Find("Answers").GetComponent<RectTransform>();
        leftColumn = answersPanel.transform.Find("Left Column").GetComponent<RectTransform>();
        rightColumn = answersPanel.transform.Find("Right Column").GetComponent<RectTransform>();

        yield return null;

        InitializeAnswers(AnswersCount);

        yield return null;

        initialized = true;
    }

    void DestroyOldAnswerObjs()
    {
        for (int i = 0; i < leftColumn.childCount; i++)
        {
            var answerObj = leftColumn.GetChild(i).gameObject;
            Destroy(answerObj);
        }

        for (int i = 0; i < rightColumn.childCount; i++)
        {
            var answerObj = rightColumn.GetChild(i).gameObject;
            Destroy(answerObj);
        }
    }

    IEnumerator LoadQuestionCoroutine(ISimpleQuestion question)
    {
        yield return new WaitUntil(() => initialized);
        yield return new WaitForEndOfFrame();

        questionText.text = question.Text;

        DisableAnswers();
        HideAllAnswers();

        yield return StartCoroutine(WaitUntilAnswersAreHiddenCoroutine());

        if (question.Answers.Length != AnswersCount)
        {
            yield return null;

            DestroyOldAnswerObjs();

            yield return null;

            InitializeAnswers(question.Answers.Length);

            yield return null;
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

    public void ChangeAnswersCount(int count)
    {
        if (this.answersButtons.Length == count)
        {
            return;
        }

        for (int i = 0; i < answersButtons.Length; i++)
        {
            var answerObj = answersButtons[i].gameObject;
            Destroy(answerObj);
        }

        InitializeAnswers(count);
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