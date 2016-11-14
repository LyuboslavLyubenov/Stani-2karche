using UnityEngine;

public class LanguageLoader : ExtendedMonoBehaviour
{
    void Start()
    {
        CoroutineUtils.WaitForFrames(0, () =>
            {
                var language = PlayerPrefsEncryptionUtils.GetString("Language");
                LanguagesManager.Instance.LoadLanguage(language);
            });
    }
}
