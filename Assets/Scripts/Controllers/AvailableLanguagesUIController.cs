using UnityEngine;
using System.IO;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Linq;
using System.Collections;
using UnityEditor;

public class AvailableLanguagesUIController : MonoBehaviour
{
    public Transform Content;
    public GameObject LoadingUI;

    const float LanguageBtnWidth = 200f;

    void Start()
    {
        var availableLanguages = GetAvailableLanguages();
        var contentRectTransform = Content.GetComponent<RectTransform>();
        var contentHeight = contentRectTransform.sizeDelta.y;

        for (int i = 0; i < availableLanguages.Length; i++)
        {
            var language = availableLanguages[i];
            AddLanguageObj(language);
        }

        var xSize = (Content.childCount * LanguageBtnWidth) + 10;
        contentRectTransform.sizeDelta = new Vector2(xSize, contentRectTransform.sizeDelta.y);
    }

    void AddLanguageObj(string language)
    {
        var obj = new GameObject();
        var textObj = new GameObject();

        obj.name = language;
        textObj.name = language + " Text";

        var rectTransform = obj.AddComponent<RectTransform>();

        rectTransform.anchorMin = new Vector2();
        rectTransform.anchorMax = new Vector2(0f, 1f);
        rectTransform.sizeDelta = new Vector2(LanguageBtnWidth, 1f);

        var textRectTransform = textObj.AddComponent<RectTransform>();

        textRectTransform.anchorMin = new Vector2();
        textRectTransform.anchorMax = new Vector2(1f, 1f);
        textRectTransform.sizeDelta = new Vector2(1f, 1f);

        var textComponent = textObj.AddComponent<Text>();

        textComponent.font = Font.CreateDynamicFontFromOSFont("Arial", 14);
        textComponent.color = Color.black;
        textComponent.text = language;
        textComponent.resizeTextForBestFit = true;
        textComponent.alignment = TextAnchor.MiddleCenter;

        obj.transform.SetParent(Content, false);
        textObj.transform.SetParent(obj.transform, false);

        var xPos = (Content.childCount * (LanguageBtnWidth + 10)) - (LanguageBtnWidth / 2);
        rectTransform.anchoredPosition = new Vector2(xPos, 0f);

        var img = obj.AddComponent<Image>();
        img.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd"); 
        img.type = Image.Type.Sliced;
        img.fillCenter = true;

        var btn = obj.AddComponent<Button>();
        btn.image = img;
        btn.onClick.AddListener(new UnityAction(() => OnSelectedLanguage(language)));
    }

    string[] GetAvailableLanguages()
    {
        string[] availableLanguages;

        #if UNITY_STANDALONE

        availableLanguages = Directory.GetFiles("Languages")
            .Where(l => Path.GetExtension(l) == ".xml")
            .Select(l => Path.GetFileNameWithoutExtension(l))
            .ToArray();

        #else

        availableLanguages = Resources.LoadAll<TextAsset>("Languages")
        .Where(l => Path.GetExtension(l.text) == ".xml")
        .Select(l => Path.GetFileNameWithoutExtension(l.text))
        .ToArray();

        #endif

        return availableLanguages;
    }

    void OnSelectedLanguage(string language)
    {
        LanguagesManager.Instance.LoadLanguage(language);
        gameObject.SetActive(false);
    }
}