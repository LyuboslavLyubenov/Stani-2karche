using UnityEngine;

public class LanguageLoader : ExtendedMonoBehaviour
{
    void Start()
    {
        CoroutineUtils.WaitForFrames(0, () =>
            {
                var language = PlayerPrefs.GetString("Language");
                LanguagesManager.Instance.LoadLanguage(language);
            });
    }
}
