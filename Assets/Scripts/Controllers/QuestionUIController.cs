using System;
using System.Collections;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Controllers
{

    using Assets.Scripts.EventArgs;
    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Utils;

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

        GameObject answerPrefab;

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
            this.OnAnswerClick = delegate
                {
                };

            this.OnQuestionLoaded = delegate
                {
                };
        }

        void Start()
        {
            this.answerPrefab = Resources.Load<GameObject>("Prefabs/Answer");
            this.StartCoroutine(this.InitializeCoroutine());
        }

        void InitializeAnswers(int answersCount)
        {
            var answers = this.GenerateAnswers(answersCount);
            this.LoadAnswersComponents(answers);
            this.AnswersCount = answersCount;
        }

        void LoadAnswersComponents(GameObject[] answers)
        {
            this.answersTexts = new Text[answers.Length];
            this.answersButtons = new Button[answers.Length];
            this.answersAnimators = new Animator[answers.Length];

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

                this.answersButtons[i] = answerButton;
                this.answersTexts[i] = answerText;
                this.answersAnimators[i] = answerAnimator;
            }
        }

        GameObject[] GenerateAnswers(int count)
        {
            var answers = new GameObject[count];

            for (int i = 0; i < answers.Length; i++)
            {
                var parent = (i % 2 == 0) ? this.leftColumn : this.rightColumn;
                var parentRectTransform = (i % 2 == 0) ? this.leftColumn : this.rightColumn;
                var parentHeight = parentRectTransform.rect.size.y;
                var answerObjsInColumn = (int)Math.Ceiling(count / 2d);
                var distanceBetweenAnswersInColumnSum = (DistanceBetweenAnswerButton / 2 * answerObjsInColumn);
                var sizeY = ((parentHeight - distanceBetweenAnswersInColumnSum) / answerObjsInColumn);
                var y = (sizeY / 2) + (parent.childCount * (DistanceBetweenAnswerButton + (int)sizeY));
                var answer = (GameObject)Instantiate(this.answerPrefab, parent, false);
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

            this.questionText = GameObject.FindWithTag("QuestionText").GetComponent<Text>();
            this.answersPanel = this.transform.Find("Answers").GetComponent<RectTransform>();
            this.leftColumn = this.answersPanel.transform.Find("Left Column").GetComponent<RectTransform>();
            this.rightColumn = this.answersPanel.transform.Find("Right Column").GetComponent<RectTransform>();

            yield return null;

            this.InitializeAnswers(this.AnswersCount);

            yield return null;

            this.initialized = true;
        }

        void DestroyOldAnswerObjs()
        {
            for (int i = 0; i < this.leftColumn.childCount; i++)
            {
                var answerObj = this.leftColumn.GetChild(i).gameObject;
                Destroy(answerObj);
            }

            for (int i = 0; i < this.rightColumn.childCount; i++)
            {
                var answerObj = this.rightColumn.GetChild(i).gameObject;
                Destroy(answerObj);
            }
        }

        IEnumerator LoadQuestionCoroutine(ISimpleQuestion question)
        {
            yield return new WaitUntil(() => this.initialized);
            yield return new WaitForEndOfFrame();

            this.questionText.text = question.Text;

            this.DisableAllAnswersInteractivity();
            this.HideAllAnswers();

            yield return this.StartCoroutine(this.WaitUntilAnswersAreHiddenCoroutine());

            if (question.Answers.Length != this.AnswersCount)
            {
                yield return null;

                this.DestroyOldAnswerObjs();

                yield return null;

                this.InitializeAnswers(question.Answers.Length);

                yield return null;
            }

            for (int i = 0; i < this.AnswersCount; i++)
            {
                var buttonIndex = i;
                var isCorrect = (buttonIndex == question.CorrectAnswerIndex);

                this.answersTexts[buttonIndex].text = question.Answers[buttonIndex];
                this.answersButtons[buttonIndex].interactable = true;
                this.answersButtons[buttonIndex].onClick.RemoveAllListeners();

                this.AttachButtonListener(buttonIndex, isCorrect);
            }

            this.ShowAllAnswers();

            this.CurrentlyLoadedQuestion = question;

            if (this.OnQuestionLoaded != null)
            {
                this.OnQuestionLoaded(this, new SimpleQuestionEventArgs(question));    
            }
        }

        IEnumerator WaitUntilAnswersAreHiddenCoroutine()
        {
            for (int i = 0; i < this.AnswersCount; i++)
            {
                while (true)
                {
                    var stateInfo = this.answersAnimators[i].GetCurrentAnimatorStateInfo(0);

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
            var button = this.answersButtons[buttonIndex];
            button.onClick.AddListener(() =>
                {
                    this.DisableAllAnswersInteractivity();

                    if (this.ShouldPlayButtonAnimation)
                    {
                        var answerText = this.answersTexts[buttonIndex].text;
                        this.ColorAnswer(answerText, true);   
                    }
                    else
                    {
                        var answer = this.answersTexts[buttonIndex].text;
                        this.OnAnswerClick(this, new AnswerEventArgs(answer, isCorrect));
                    }
                });
        }

        void ColorAnswer(string answer, bool fireClickEvent)
        {
            var correctAnswerIndex = this.CurrentlyLoadedQuestion.CorrectAnswerIndex;
            var answerIndex = this.CurrentlyLoadedQuestion.Answers.ToList().FindIndex(a => a == answer);
            var isCorrect = (correctAnswerIndex == answerIndex);
            var answerAnimator = this.answersAnimators[answerIndex];

            answerAnimator.SetTrigger("clicked");
            answerAnimator.SetBool("fireClickEvent", fireClickEvent);
            answerAnimator.SetBool("isCorrect", isCorrect);
        }

        void ShowCorrectAnswer()
        {
            var correctAnswerIndex = this.CurrentlyLoadedQuestion.CorrectAnswerIndex;
            var correctAnswer = this.CurrentlyLoadedQuestion.Answers[correctAnswerIndex];
            this.ColorAnswer(correctAnswer, false);
        }

        void ShowAnswer(int index)
        {
            if (index < 0 || index >= this.AnswersCount)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            var currentState = this.answersAnimators[index].GetCurrentAnimatorStateInfo(0);

            if (!currentState.IsTag("Visible"))
            {
                this.answersAnimators[index].SetTrigger("show");
            }
        }

        int GetAnswerIndex(string answer)
        {        
            for (int i = 0; i < this.AnswersCount; i++)
            {
                var answerText = this.answersTexts[i].text;

                if (answerText == answer)
                {
                    return i;
                }
            }

            return -1;
        }

        public void ChangeAnswersCount(int count)
        {
            if (this.answersButtons.Length == count)
            {
                return;
            }

            for (int i = 0; i < this.answersButtons.Length; i++)
            {
                var answerObj = this.answersButtons[i].gameObject;
                Destroy(answerObj);
            }

            this.InitializeAnswers(count);
        }

        public void HideAnswer(int index)
        {
            if (index < 0 || index >= this.AnswersCount)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            var currentState = this.answersAnimators[index].GetCurrentAnimatorStateInfo(0);

            if (!currentState.IsTag("Hidden"))
            {
                this.answersAnimators[index].SetTrigger("hide");    
            }
        }

        public void HideAnswer(string answer)
        {
            int answerIndex = this.GetAnswerIndex(answer);

            if (answerIndex == -1)
            {
                throw new ArgumentException("Answer doesnt exists");
            }

            this.HideAnswer(answerIndex);
        }

        public void HideAllAnswers()
        {
            for (int i = 0; i < this.AnswersCount; i++)
            {
                this.HideAnswer(i);
            }
        }

        public void ShowAllAnswers()
        {
            for (int i = 0; i < this.AnswersCount; i++)
            {
                this.ShowAnswer(i);  
            }
        }

        public void LoadQuestion(ISimpleQuestion question)
        {
            this.StartCoroutine(this.LoadQuestionCoroutine(question));
        }

        public void _OnIncorrectAnswerAnimEnd(string answer)
        {
            if (this.ShowCorrectAnswerAfterError)
            {
                this.ShowCorrectAnswer();

                //TODO: Get correct answer animation length and wait
                this.CoroutineUtils.WaitForSeconds(3f, () =>
                    {
                        this.OnAnswerClick(this, new AnswerEventArgs(answer, false));
                    });
            
            }
            else
            {
                this.OnAnswerClick(this, new AnswerEventArgs(answer, false));
            }

        }

        public void _OnCorrectAnswerAnimEnd(string answer)
        {
            this.OnAnswerClick(this, new AnswerEventArgs(answer, true));
        }

        public void DisableAnswerInteractivity(string answer)
        {
            var answerIndex = this.GetAnswerIndex(answer);

            if (answerIndex == -1)
            {
                throw new ArgumentException("Answer doesnt exists");
            }

            this.DisableAnswerInteractivity(answerIndex);
        }

        public void DisableAnswerInteractivity(int answerIndex)
        {
            this.answersButtons[answerIndex].interactable = false;
        }

        public void DisableAllAnswersInteractivity()
        {
            for (int i = 0; i < this.AnswersCount; i++)
            {
                this.DisableAnswerInteractivity(i);
            }
        }
    }

}