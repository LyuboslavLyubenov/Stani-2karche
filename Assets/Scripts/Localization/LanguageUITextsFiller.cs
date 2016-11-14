using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LanguageUITextsFiller : ExtendedMonoBehaviour
{
    void Start()
    {
        LanguagesManager.Instance.OnLoadedLanguage += (sender, args) => TranslateAllTextComponentsInScene();
        SceneManager.activeSceneChanged += OnSceneChanged;

        CoroutineUtils.WaitForFrames(1, TranslateAllTextComponentsInScene);
    }

    void OnSceneChanged(Scene oldScene, Scene newScene)
    {
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
