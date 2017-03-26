//покажи броя кутии
//цъка се на кутия
//кутията се показва посредата на контейнера
//Някакъв евент
//Чака се да се извика ShowAnswer/ShowNothing
//Маха се кутията и отговора (ако има)

namespace Controllers.Jokers
{
    using System;
    using System.Collections;
    using System.Linq;

    using Assets.Scripts.Extensions.Unity;

    using Extensions;
    using Extensions.Unity;

    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;
    
    public class KalitkoJokerContainerUIController : MonoBehaviour
    {
        private const int AnswerYOffset = 10;

        public int DistanceBetweenBox = 10;
        public int MaxBoxesOnRow = 5;
        
        [SerializeField]
        private Transform BoxPrefab;

        [SerializeField]
        private Transform AnswerPrefab;

        [SerializeField]
        private Transform ConfettiPrefab;
        
        private RectTransform rectTransform;

        public bool AnswerSelected
        {
            get;
            private set;
        }

        void Awake()
        {
            this.rectTransform = this.GetComponent<RectTransform>();
        }

        private IEnumerator CreateBoxesCoroutine(int boxCount, Action onCreatedBoxes)
        {
            yield return new WaitForSeconds(0.2f);
            
            var containerWidth = this.rectTransform.sizeDelta.x;
            var rows = (int)Math.Ceiling(boxCount / (float)MaxBoxesOnRow);
            var boxSideLength = (int)((containerWidth - this.DistanceBetweenBox) / (this.MaxBoxesOnRow)) - (this.DistanceBetweenBox);
            var createdBoxesCount = 0;

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < MaxBoxesOnRow && createdBoxesCount < boxCount; j++)
                {
                    yield return new WaitForSeconds(0.2f);
                    this.CreateBox(boxSideLength, j, i, createdBoxesCount, containerWidth);

                    createdBoxesCount++;
                }
            }

            var containerHeight = rows * (boxSideLength + this.DistanceBetweenBox);
            this.rectTransform.sizeDelta = new Vector2(containerWidth, containerHeight);

            yield return null;

            onCreatedBoxes();
        }

        private void CreateBox(int boxSideLength, int j, int i, int createdBoxesCount, float containerWidth)
        {
            var boxInstance = Instantiate(this.BoxPrefab, Vector3.zero, Quaternion.identity, this.transform);
            var boxRectTransform = boxInstance.GetComponent<RectTransform>();

            boxRectTransform.sizeDelta = new Vector2(boxSideLength, boxSideLength);

            var x = this.DistanceBetweenBox + (boxSideLength / 2) + (j * (this.DistanceBetweenBox + boxSideLength));
            var y = this.DistanceBetweenBox + (boxSideLength / 2) + (i * (this.DistanceBetweenBox + boxSideLength));
            boxRectTransform.anchoredPosition = new Vector2(x, -y);

            boxInstance.GetComponentInChildren<Text>().text = createdBoxesCount.ToString();

            boxInstance.GetComponent<Button>()
                .onClick
                .AddListener(this.ClickedOnBox);
        }

        private IEnumerator DestroyNotSelectedBoxes(int selectedBoxNumber)
        {
            var boxesCount = this.transform.childCount;

            for (int i = 0; i < boxesCount; i++)
            {
                yield return new WaitForSeconds(0.2f);

                var box = this.transform.GetChild(i);
                var boxNumber = box.GetComponentInChildren<Text>()
                    .text
                    .ConvertTo<int>();

                if (boxNumber != selectedBoxNumber)
                {
                    box.GetComponent<Animator>().SetTrigger("destroy");
                }
            }

            yield return new WaitForSeconds(2f);
        }
 
        private IEnumerator ShowBoxOnCenter(GameObject boxObj)
        {
            var boxRectTransform = boxObj.GetComponent<RectTransform>();
            var beginPosition = new Vector3(boxRectTransform.anchoredPosition.x, boxRectTransform.anchoredPosition.y, 0);
            var endPosition = new Vector3(0, -(boxRectTransform.sizeDelta.y / 2f), 0);

            for (float i = 0; i <= 1f; i += 0.1f)
            {
                yield return new WaitForEndOfFrame();

                var newPosition = Vector3.Slerp(beginPosition, endPosition, i);
                boxRectTransform.anchoredPosition = new Vector2(newPosition.x, newPosition.y);
            }

            boxRectTransform.anchoredPosition = endPosition;
        }

        private IEnumerator ClickedOnBoxCoroutine()
        {
            var boxObj = EventSystem.current.currentSelectedGameObject;
            var number = boxObj.GetComponentInChildren<Text>()
                .text
                .ConvertTo<int>();

            yield return StartCoroutine(this.DestroyNotSelectedBoxes(number));
            yield return StartCoroutine(this.ShowBoxOnCenter(boxObj));

            this.AnswerSelected = true;
        }

        private void ClickedOnBox()
        {
            this.StartCoroutine(this.ClickedOnBoxCoroutine());
        }

        private IEnumerator ShowAnswerCoroutine(string answer)
        {
            yield return this.StartCoroutine(this.OpenCurrentlySelectedBox());
            
            var confettiObj = Instantiate(this.ConfettiPrefab, this.transform, false);
            confettiObj.localScale = new Vector3(1, 1, 1);
            confettiObj.transform.position = new Vector3();

            var answerObj = Instantiate(this.AnswerPrefab, this.transform, false);

            var answerRectTransform = answerObj.GetComponent<RectTransform>();
            answerRectTransform.anchoredPosition = new Vector2(0, (answerRectTransform.sizeDelta.y / 2f) + AnswerYOffset);

            answerObj.GetComponentInChildren<Text>()
                .text = answer;

            yield return new WaitForSeconds(7f);

            this.Clean();
            this.gameObject.SetActive(false);
        }

        private IEnumerator ShowNothingCoroutine()
        {
            yield return this.StartCoroutine(this.OpenCurrentlySelectedBox());
            yield return new WaitForSeconds(3f);
            this.Clean();
            this.gameObject.SetActive(false);
        }

        private IEnumerator OpenCurrentlySelectedBox()
        {
            yield return null;

            var boxObj = this.transform.GetChild(0);
            var boxAnimator = boxObj.GetComponent<Animator>();

            boxAnimator.SetTrigger("open");

            yield return new WaitForSeconds(3f);
        }

        private void Clean()
        {
            this.transform.GetAllChildren()
                .ToList()
                .ForEach(c => Destroy(c.gameObject));
            
            this.AnswerSelected = false;
        }

        public void CreateBoxes(int boxCount, Action onCreatedBoxes)
        {
            if (this.AnswerSelected)
            {
                throw new InvalidOperationException("Already created boxes");
            }

            if (boxCount <= 0)
            {
                throw new ArgumentOutOfRangeException("boxCount");
            }

            if (onCreatedBoxes == null)
            {
                throw new ArgumentNullException("onCreatedBoxes");
            }
              
            this.StartCoroutine(this.CreateBoxesCoroutine(boxCount, onCreatedBoxes));
        }

        public void ShowAnswer(string answer)
        {
            if (!this.AnswerSelected)
            {
                throw new InvalidOperationException("Must call CreateBoxes before that");
            }

            if (string.IsNullOrEmpty(answer))
            {
                throw new ArgumentNullException("answer");
            }

            this.StartCoroutine(this.ShowAnswerCoroutine(answer));
        }

        public void ShowNothing()
        {
            if (!this.AnswerSelected)
            {
                throw new InvalidOperationException("Must call CreateBoxes before that");
            }

            this.StartCoroutine(this.ShowNothingCoroutine());
        }
    }
}