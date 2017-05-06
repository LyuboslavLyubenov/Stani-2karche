namespace Controllers
{

    using System;
    using System.Collections.Generic;

    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Interfaces.Controllers;

    using EventArgs;

    using Interfaces;

    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    public class AvailableJokersUIController : MonoBehaviour, IAvailableJokersUIController
    {
        private const int SpawnOffset = 10;

        public event EventHandler<JokerEventArgs> OnAddedJoker = delegate
            {
            };

        public event EventHandler<JokerEventArgs> OnUsedJoker = delegate
            {
            };

        public Transform Container;

        public int JokersCount
        {
            get
            {
                return this.jokerObjs.Count;
            }
        }

        private RectTransform containerRectTransform;

        private List<Transform> jokerObjs = new List<Transform>();
        private List<IJoker> jokers = new List<IJoker>();

        private Transform dummyJokerButtonPrefab;

        private Vector2 jokerStartPosition;
        private Vector2 jokerButtonSize;

        private float distanceBetweenJokers = 0;

        // ReSharper disable once ArrangeTypeMemberModifiers
        void Start()
        {
            this.containerRectTransform = this.Container.GetComponent<RectTransform>();
            this.dummyJokerButtonPrefab = Resources.Load<Transform>("Prefabs/DummyJokerButton");

            var rectTransform = this.dummyJokerButtonPrefab.GetComponent<RectTransform>();
            this.jokerButtonSize = rectTransform.sizeDelta;
            this.jokerStartPosition = rectTransform.anchoredPosition;
            this.distanceBetweenJokers = this.jokerStartPosition.y - this.jokerButtonSize.y;
        }

        protected virtual void OnJokerClick(GameObject jokerObj, IJoker joker)
        {
            joker.Activate();

            this.OnUsedJoker(this, new JokerEventArgs(joker));

            this.jokers.Remove(joker);
            Destroy(jokerObj);
        }

        public void AddJoker(IJoker joker)
        {
            if (joker == null)
            {
                throw new ArgumentNullException("joker");
            }

            var jokerObj = Instantiate(this.dummyJokerButtonPrefab);

            jokerObj.name = joker.GetType().Name.Replace("Joker", "");
            jokerObj.SetParent(this.Container, false);

            var jokerRect = jokerObj.GetComponent<RectTransform>();
            var x = this.jokerStartPosition.x;
            var y = this.jokerStartPosition.y + SpawnOffset + (this.jokers.Count * (this.jokerButtonSize.y + this.distanceBetweenJokers));

            jokerRect.anchoredPosition = new Vector2(x, y);

            var jokerButton = jokerObj.GetComponent<Button>();
            jokerButton.onClick.AddListener(new UnityAction(() => this.OnJokerClick(jokerObj.gameObject, joker)));

            var jokerImageObj = jokerObj.GetChild(0);
            var jokerImage = jokerImageObj.GetComponent<Image>();
            jokerImage.sprite = joker.Image;

            this.jokerObjs.Add(jokerObj);
            this.jokers.Add(joker);

            var contentHeight = -this.jokerStartPosition.y + (this.jokers.Count * (this.jokerButtonSize.y + 10));
            this.containerRectTransform.sizeDelta = new Vector2(this.containerRectTransform.sizeDelta.x, contentHeight);

            this.OnAddedJoker(this, new JokerEventArgs(joker));
        }

        public void ClearAll()
        {
            for (int i = 0; i < this.jokerObjs.Count; i++)
            {
                var jokerObj = this.jokerObjs[i];

                if (jokerObj == null)
                {
                    continue;
                }

                Destroy(jokerObj.gameObject);
            }

            this.jokers.Clear();
            this.jokerObjs.Clear();
        }
    }

}