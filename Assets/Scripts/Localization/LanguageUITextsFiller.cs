using UnityEngine;
using UnityEngine.UI;

public class LanguageUITextsFiller : ExtendedMonoBehaviour
{
    void Start()
    {
        LanguagesManager.Instance.OnLanguageLoad += (sender, args) => TranslateAllTextComponentsInScene();
        CoroutineUtils.WaitForFrames(1, TranslateAllTextComponentsInScene);
    }

    void TranslateAllTextComponentsInScene()
    {
        if (!LanguagesManager.Instance.IsLoadedLanguage)
        {
            return;  
        }

        var allObjectsInScene = GameObjectUtils.GetAllObjectsIncludingInactive();

        for (var i = 0; i < allObjectsInScene.Length; i++)
        {
            var obj = allObjectsInScene[i];
            var textComponent = obj.GetComponent<Text>();

            if (textComponent == null)
            {
                continue;
            }

            var path = textComponent.text;
            var translated = LanguagesManager.Instance.GetValue(path);

            textComponent.text = translated;
        }
    }
}
