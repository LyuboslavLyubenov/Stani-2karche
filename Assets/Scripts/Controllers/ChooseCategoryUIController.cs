using UnityEngine;
using System.IO;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using System;
using UnityEngine.Events;

public class ChooseCategoryUIController : MonoBehaviour
{
    const int SpaceBetweenElements = 10;

    readonly string[] RequiredFiles = new string[] { "3.xls", "4.xls", "5.xls", "6.xls", "Rating.csv" };

    public Transform ContentPanel;

    public EventHandler<ChoosedCategoryEventArgs> OnChoosedCategory = delegate
    {
    };

    GameObject categoryElementPrefab;

    bool IsValidLevel(string path)
    {
        var files = Directory.GetFiles(path).Select(f => f.Substring(path.Length + 1)).ToArray(); 
        var isValidLevel = files.All(f => RequiredFiles.Contains(f));
        return isValidLevel;
    }

    IEnumerator InitializeCoroutine()
    {
        categoryElementPrefab = Resources.Load<GameObject>("Prefabs/CategoryToSelectButton");

        var currentDirectory = Directory.GetCurrentDirectory() + "\\LevelData\\теми\\";
        var availableCategories = Directory.GetDirectories(currentDirectory).Where(IsValidLevel).ToArray();

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
                elementText.text = availableCategories[i].Remove(0, currentDirectory.Length);

                var elementButton = categoryElement.GetComponent<Button>();
                elementButton.onClick.AddListener(new UnityAction(() => ChooseCategory(availableCategories[i])));

                yield return new WaitForSeconds(0.05f);
            }

            var elementsCount = ContentPanel.childCount;
            var lastElement = ContentPanel.GetChild(elementsCount - 1);
            var lastElementRectTransform = lastElement.GetComponent<RectTransform>();
            var contentRectTransform = ContentPanel.GetComponent<RectTransform>();

            var width = lastElementRectTransform.localPosition.x + (elementWidth / 2) + SpaceBetweenElements;
            var height = contentRectTransform.sizeDelta.y;

            contentRectTransform.sizeDelta = new Vector2(width, height);
        }
    }

    void ChooseCategory(string path)
    {
        var button = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        var categoryName = button.GetComponentInChildren<Text>().text;

        OnChoosedCategory(this, new ChoosedCategoryEventArgs(categoryName, path));

        gameObject.SetActive(false);
    }

    public void Initialize()
    {
        StartCoroutine(InitializeCoroutine());
    }
}