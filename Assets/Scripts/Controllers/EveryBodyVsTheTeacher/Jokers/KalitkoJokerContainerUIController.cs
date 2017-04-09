//покажи броя кутии
//цъка се на кутия
//кутията се показва посредата на контейнера
//Някакъв евент
//Чака се да се извика ShowAnswer/ShowNothing
//Маха се кутията и отговора (ако има)

namespace Controllers.EveryBodyVsTheTeacher.Jokers
{

    using System;
    using System.Collections;
    using System.Linq;

    using Extensions.Unity;

    using UnityEngine;
    using UnityEngine.UI;

    public class KalitkoJokerContainerUIController : MonoBehaviour
    {
        private const int AnswerYOffset = 60;
        private const int ConfettiYOffset = 2;
        
        [SerializeField]
        private Transform BoxPrefab;

        [SerializeField]
        private Transform AnswerPrefab;

        [SerializeField]
        private Transform ConfettiPrefab;

        private Transform CreateBox()
        {
            var boxInstance = Instantiate(this.BoxPrefab, Vector3.zero, Quaternion.identity, this.transform);
            var boxRectTransform = boxInstance.GetComponent<RectTransform>();

            boxRectTransform.anchoredPosition = new Vector2();

            return boxInstance;
        }

        private void OpenBox(Transform boxObj)
        {
            var boxAnimator = boxObj.GetComponent<Animator>();
            boxAnimator.SetTrigger("open");
        }

        private IEnumerator ShowAnswerCoroutine(string answer)
        {
            var boxObj = this.CreateBox();
            this.OpenBox(boxObj);

            yield return new WaitForSeconds(3.5f);

            var confettiObj = Instantiate(this.ConfettiPrefab, this.transform, false);
            confettiObj.localScale = new Vector3(1, 1, 1);
            confettiObj.transform.position = new Vector3(0, ConfettiYOffset);

            var answerObj = Instantiate(this.AnswerPrefab, this.transform, false);

            var answerRectTransform = answerObj.GetComponent<RectTransform>();
            answerRectTransform.anchoredPosition = new Vector2(0, (answerRectTransform.sizeDelta.y / 2f) + AnswerYOffset);

            answerObj.GetComponentInChildren<Text>()
                .text = answer;

            yield return new WaitForSeconds(7.5f);

            this.Clean();
            this.gameObject.SetActive(false);
        }

        private IEnumerator ShowNothingCoroutine()
        {
            var boxInstance = this.CreateBox();
            this.OpenBox(boxInstance);

            yield return new WaitForSeconds(5.5f);

            this.Clean();
            this.gameObject.SetActive(false);
        }
        
        private void Clean()
        {
            this.transform.GetAllChildren()
                .ToList()
                .ForEach(c => Destroy(c.gameObject));
        }
        
        public void ShowAnswer(string answer)
        {
            if (string.IsNullOrEmpty(answer))
            {
                throw new ArgumentNullException("answer");
            }

            this.StartCoroutine(this.ShowAnswerCoroutine(answer));
        }

        public void ShowNothing()
        {
            this.StartCoroutine(this.ShowNothingCoroutine());
        }
    }
}