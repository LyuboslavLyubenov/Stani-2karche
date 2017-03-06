namespace Assets.Scripts.Controllers
{
    using System;
    using System.Collections;

    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    using EventArgs;
    using Interfaces;
    using Network.NetworkManagers;
    using Notifications;

    using EventArgs = System.EventArgs;

    public class ClientChooseCategoryUIController : MonoBehaviour
    {
        private const int SpaceBetweenElements = 10;

        public EventHandler OnLoadedCategories = delegate
            {
            };

        public EventHandler<ChoosedCategoryEventArgs> OnChoosedCategory = delegate
            {
            };

        public ClientNetworkManager NetworkManager;
        public Transform ContentPanel;
        public NotificationsesController Notificationse;

        private GameObject categoryElementPrefab;

        private IEnumerator InitializeCoroutine(string[] availableCategories)
        {
            this.categoryElementPrefab = Resources.Load<GameObject>("Prefabs/CategoryToSelectButton");

            if (availableCategories.Length > 0)
            {
                var elementWidth = this.categoryElementPrefab.GetComponent<RectTransform>().sizeDelta.x;

                for (int i = 0; i < availableCategories.Length; i++)
                {
                    var categoryElement = Instantiate(this.categoryElementPrefab);
                    var elementRectTransform = categoryElement.GetComponent<RectTransform>();

                    categoryElement.transform.SetParent(this.ContentPanel, false);

                    var x = (elementWidth / 2) + SpaceBetweenElements + (i * (elementWidth + SpaceBetweenElements));
                    var y = elementRectTransform.anchoredPosition.y;

                    elementRectTransform.anchoredPosition = new Vector2(x, y);

                    var elementText = categoryElement.GetComponentInChildren<Text>();
                    elementText.text = availableCategories[i];

                    var elementButton = categoryElement.GetComponent<Button>();
                    elementButton.onClick.AddListener(new UnityAction(() => this.ChoosedCategory()));

                    yield return new WaitForSeconds(0.1f);
                    yield return new WaitForEndOfFrame();
                }

                var elementsCount = this.ContentPanel.childCount;
                var lastElement = this.ContentPanel.GetChild(elementsCount - 1);
                var lastElementRectTransform = lastElement.GetComponent<RectTransform>();
                var contentRectTransform = this.ContentPanel.GetComponent<RectTransform>();

                var width = lastElementRectTransform.localPosition.x + (elementWidth / 2) + SpaceBetweenElements;
                var height = contentRectTransform.sizeDelta.y;

                contentRectTransform.sizeDelta = new Vector2(width, height);
            }

            this.OnLoadedCategories(this, EventArgs.Empty);
        }

        private void ChoosedCategory()
        {
            var button = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
            var categoryName = button.GetComponentInChildren<Text>().text;

            this.OnChoosedCategory(this, new ChoosedCategoryEventArgs(categoryName));
           
            this.gameObject.SetActive(false);
        }

        private void RemoveOldCategories()
        {
            var childCount = this.ContentPanel.childCount;

            for (int i = 0; i < childCount; i++)
            {
                var categoryObj = this.ContentPanel.GetChild(i);
                Destroy(categoryObj);
            }
        }

        public void Initialize(IAvailableCategoriesReader categoriesReader)
        {
            this.RemoveOldCategories();
            categoriesReader.GetAllCategories((categories) => this.StartCoroutine(this.InitializeCoroutine(categories)));
        }
    }
}