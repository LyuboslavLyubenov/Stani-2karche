using UnityEngine;
using System.IO;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using System;

public class ChooseThemeUIController : MonoBehaviour
{
    const int SpaceBetweenElements = 10;

    readonly string[] RequiredFiles = new string[] { "3.xls", "4.xls", "5.xls", "6.xls", "Rating.csv" };

    public Transform ContentPanel;
    public GameData GameData;
    public LeaderboardSerializer LeaderboardSerializer;

    public EventHandler OnChoosedTheme = delegate
    {
    };

    GameObject themeElementPrefab;

    bool IsValidLevel(string path)
    {
        var files = Directory.GetFiles(path).Select(f => f.Substring(path.Length + 1)).ToArray(); 
        var isValidLevel = files.All(f => RequiredFiles.Contains(f));
        return isValidLevel;
    }

    IEnumerator InitializeCoroutine()
    {
        themeElementPrefab = Resources.Load<GameObject>("Prefabs/ThemeToSelectButton");

        var currentDirectory = Directory.GetCurrentDirectory() + "\\LevelData\\теми\\";
        var availableThemes = Directory.GetDirectories(currentDirectory).Where(IsValidLevel).ToArray();

        if (availableThemes.Length > 0)
        {
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
                elementButton.onClick.AddListener(ChooseTheme);

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

    void ChooseTheme()
    {
        var button = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        var categoryName = button.GetComponentInChildren<Text>().text;

        GameData.LevelCategory = categoryName;
        GameData.LoadDataAsync();

        LeaderboardSerializer.LevelCategory = categoryName;
        LeaderboardSerializer.LoadDataAsync();

        OnChoosedTheme(this, EventArgs.Empty);

        gameObject.SetActive(false);
    }

    public void Initialize()
    {
        StartCoroutine(InitializeCoroutine());
    }
}
