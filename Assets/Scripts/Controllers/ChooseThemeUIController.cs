using UnityEngine;
using System.IO;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ChooseThemeUIController : MonoBehaviour
{
    const int SpaceBetweenElements = 10;

    public GameObject LoadingUI;
    public GameObject BasicExamPlaygroundUI;
    public Transform ContentPanel;
    public GameData GameData;
    public LeaderboardSerializer LeaderboardSerializer;

    GameObject themeElementPrefab;

    IEnumerator InitializeCoroutine()
    {
        themeElementPrefab = Resources.Load<GameObject>("Prefabs/ThemeToSelectButton");

        var currentDirectory = Directory.GetCurrentDirectory() + "\\LevelData\\теми\\";
        var availableThemes = Directory.GetDirectories(currentDirectory);
        var elementWidth = themeElementPrefab.GetComponent<RectTransform>().sizeDelta.x;

        for (int i = 0; i < availableThemes.Length; i++)
        {
            var themeElement = Instantiate(themeElementPrefab);
            var elementRectTransform = themeElement.GetComponent<RectTransform>();

            themeElement.transform.SetParent(ContentPanel, false);

            var x = (elementWidth / 2) + SpaceBetweenElements + (i * (elementWidth + SpaceBetweenElements));
            var y = elementRectTransform.anchoredPosition.y;

            elementRectTransform.anchoredPosition = new Vector2(x, y);

            var elementText = themeElement.GetComponentInChildren<Text>();
            elementText.text = availableThemes[i].Remove(0, currentDirectory.Length);

            var elementButton = themeElement.GetComponent<Button>();
            elementButton.onClick.AddListener(OnChoosedTheme);

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

    void OnChoosedTheme()
    {
        var button = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        var categoryName = button.GetComponentInChildren<Text>().text;

        GameData.LevelCategory = categoryName;
        GameData.LoadDataAsync();

        LeaderboardSerializer.LevelCategory = categoryName;
        LeaderboardSerializer.LoadDataAsync();
           
        LoadingUI.SetActive(true);
        BasicExamPlaygroundUI.SetActive(true);
        gameObject.SetActive(false);
    }

    public void Initialize()
    {
        StartCoroutine(InitializeCoroutine());
    }
}
