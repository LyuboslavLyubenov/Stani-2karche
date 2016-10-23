using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using UnityEngine.Events;

public class ClientChooseCategoryUIController : MonoBehaviour
{
    const int SpaceBetweenElements = 10;

    public EventHandler OnLoadedCategories = delegate
    {
    };

    public EventHandler<ChoosedCategoryEventArgs> OnChoosedCategory = delegate
    {
    };

    public ClientNetworkManager NetworkManager;
    public Transform ContentPanel;
    public NotificationsServiceController NotificationService;

    GameObject categoryElementPrefab;

    IEnumerator InitializeCoroutine(string[] availableCategories)
    {
        categoryElementPrefab = Resources.Load<GameObject>("Prefabs/CategoryToSelectButton");

        if (availableCategories.Length > 0)
        {
            var elementWidth = categoryElementPrefab.GetComponent<RectTransform>().sizeDelta.x;

            for (int i = 0; i < availableCategories.Length; i++)
            {
                var categoryElement = Instantiate(categoryElementPrefab);
                var elementRectTransform = categoryElement.GetComponent<RectTransform>();

                categoryElement.transform.SetParent(ContentPanel, false);

                var x = (elementWidth / 2) + SpaceBetweenElements + (i * (elementWidth + SpaceBetweenElements));
                var y = elementRectTransform.anchoredPosition.y;

                elementRectTransform.anchoredPosition = new Vector2(x, y);

                var elementText = categoryElement.GetComponentInChildren<Text>();
                elementText.text = availableCategories[i];

                var elementButton = categoryElement.GetComponent<Button>();
                elementButton.onClick.AddListener(new UnityAction(() => ChoosedCategory()));

                yield return new WaitForSeconds(0.05f);
                yield return new WaitForEndOfFrame();
            }

            var elementsCount = ContentPanel.childCount;
            var lastElement = ContentPanel.GetChild(elementsCount - 1);
            var lastElementRectTransform = lastElement.GetComponent<RectTransform>();
            var contentRectTransform = ContentPanel.GetComponent<RectTransform>();

            var width = lastElementRectTransform.localPosition.x + (elementWidth / 2) + SpaceBetweenElements;
            var height = contentRectTransform.sizeDelta.y;

            contentRectTransform.sizeDelta = new Vector2(width, height);
        }

        OnLoadedCategories(this, EventArgs.Empty);
    }

    void ChoosedCategory()
    {
        var button = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        var categoryName = button.GetComponentInChildren<Text>().text;

        OnChoosedCategory(this, new ChoosedCategoryEventArgs(categoryName));
           
        gameObject.SetActive(false);
    }

    public void Initialize(IAvailableCategoriesReader categoriesReader)
    {
        categoriesReader.GetAllCategories((categories) => StartCoroutine(InitializeCoroutine(categories)));
    }
}