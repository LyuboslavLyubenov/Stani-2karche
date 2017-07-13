namespace Controllers.Jokers
{

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using Assets.Scripts.Utils;

    using EventArgs;

    using UnityEngine;
    using UnityEngine.UI;

    using Utils.Unity;

    public class SelectRandomJokerUIController : ExtendedMonoBehaviour
    {
        public EventHandler<JokerTypeEventArgs> OnFinishedAnimation = delegate
            {
            };

        private const float DelayBetweenJokersRemovalInSeconds = 0.9f;
        private const float DelayShowSelectedJoker = 3f;
        private const int XDistanceBetweenJokers = 20;
        private const int YOffset = 20;

        private Dictionary<Type, GameObject> jokersObjs = new Dictionary<Type, GameObject>();

        private RectTransform upperContainer;
        private RectTransform lowerContainer;
        private RectTransform thisRectTransform;

        private GameObject jokerPrefab;

        // ReSharper disable once ArrangeTypeMemberModifiers
        void Start()
        {
            this.upperContainer = this.transform.Find("Upper").GetComponent<RectTransform>();
            this.lowerContainer = this.transform.Find("Lower").GetComponent<RectTransform>();
            this.thisRectTransform = this.transform.GetComponent<RectTransform>();
            this.jokerPrefab = Resources.Load<GameObject>("Prefabs/SelectRandomJoker/Joker");
        }

        private IEnumerator CreateJokerObjs(Type[] jokers)
        {
            yield return null;

            this.jokersObjs.Clear();
            
            for (int i = 0; i < jokers.Length; i++)
            {
                var joker = jokers[i];
                var parent = (i % 2 == 0) ? this.upperContainer : this.lowerContainer;
                var obj = Instantiate(this.jokerPrefab, parent, false);
                var jokerName = joker.Name.Replace("Joker", "");

                obj.name = jokerName;

                var jokerImagePath = JokerConstants.JokerImagesPath + jokerName;
                var jokerImage = Resources.Load<Sprite>(jokerImagePath);
                var imgObj = obj.transform.GetChild(0);

                imgObj.GetComponent<Image>().sprite = jokerImage;

                var rectTransform = obj.GetComponent<RectTransform>();
                var parentJokersCount = (int)Math.Ceiling(jokers.Length / 2f);
                var xSize = ((parent.rect.width - (parentJokersCount * XDistanceBetweenJokers)) / parentJokersCount);
                var ySize = parent.rect.height - YOffset;
                var size = new Vector2(xSize, ySize);

                rectTransform.sizeDelta = size;

                var xPos = (xSize / 2f) + (XDistanceBetweenJokers / 2f) + (xSize + XDistanceBetweenJokers) * (parent.childCount - 1);
                var pos = new Vector2(xPos, 0);

                rectTransform.anchoredPosition = pos;

                this.jokersObjs.Add(joker, obj);

                yield return null;
            }
        }

        private IEnumerator DeleteNotSelectedJokers(Type[] notSelectedJokers)
        {
            var jokersToDelete = notSelectedJokers.ToArray();
            jokersToDelete.Shuffle();

            for (int i = 0; i < jokersToDelete.Length; i++)
            {
                yield return new WaitForEndOfFrame();

                var joker = jokersToDelete[i];
                var obj = this.jokersObjs[joker];

                obj.GetComponent<Animator>().SetTrigger("destroy");
                this.jokersObjs.Remove(joker);

                yield return new WaitForSeconds(DelayBetweenJokersRemovalInSeconds);
            }
        }

        private IEnumerator PlaySelectRandomJokerCoroutine(Type[] jokers, int selectedJokerIndex)
        {
            yield return this.StartCoroutine(this.CreateJokerObjs(jokers));
            yield return new WaitForEndOfFrame();

            var selectedJoker = jokers[selectedJokerIndex];
            var jokersToDelete = jokers.Where(j => j != selectedJoker).ToArray();
         
            yield return this.StartCoroutine(this.DeleteNotSelectedJokers(jokersToDelete));
            yield return new WaitForSeconds(1);
            yield return this.StartCoroutine(this.ShowAsSelectedCoroutine(selectedJoker));
        }

        private IEnumerator ShowAsSelectedCoroutine(Type joker)
        {
            var selectedJokerObj = this.jokersObjs[joker];
            selectedJokerObj.transform.SetParent(this.transform, true);

            yield return null;

            var rectTransform = selectedJokerObj.GetComponent<RectTransform>();
            var start = new Vector3(rectTransform.anchoredPosition.x, rectTransform.anchoredPosition.y, 0);
            var endX = this.thisRectTransform.rect.width / 2;
            var end = new Vector3(endX, 0, 0);

            for (float i = 0f; i <= 1f; i += 0.075f)
            {
                var nextPosition = Vector3.Slerp(start, end, i);
                rectTransform.anchoredPosition = new Vector2(nextPosition.x, nextPosition.y);
                yield return null;
            }

            yield return new WaitForSeconds(DelayShowSelectedJoker);

            Destroy(selectedJokerObj);

            yield return new WaitForSeconds(1f);

            this.OnFinishedAnimation(this, new JokerTypeEventArgs(joker));

            this.gameObject.SetActive(false);
        }

        public void PlaySelectRandomJokerAnimation(Type[] jokers, int selectedJokerIndex)
        {
            if (jokers == null ||
                jokers.Length < 1)
            {
                throw new ArgumentException("jokers");
            }

            if (selectedJokerIndex < 0 || selectedJokerIndex >= jokers.Length)
            {
                throw new ArgumentOutOfRangeException("selectedJokerIndex");
            }

            this.StartCoroutine(this.PlaySelectRandomJokerCoroutine(jokers, selectedJokerIndex));
        }
    }

}