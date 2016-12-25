﻿namespace Assets.Scripts.Localization
{

    using Assets.Scripts.Utils;

    public class LanguageLoader : ExtendedMonoBehaviour
    {
        void Start()
        {
            this.CoroutineUtils.WaitForFrames(0, () =>
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

}
