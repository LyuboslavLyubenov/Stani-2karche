using UnityEngine;

public class LanguageLoader : ExtendedMonoBehaviour
{
    void Start()
    {
        CoroutineUtils.WaitForFrames(0, () =>
            {
                var language = "Bulgarian";

                if (PlayerPrefsEncryptionUtils.HasKey("Language"))
                {
                    language = PlayerPrefsEncryptionUtils.GetString("Language");    
                }

                LanguagesManager.Instance.LoadLanguage(language);
            });
    }
}
