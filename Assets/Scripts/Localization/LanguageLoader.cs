namespace Localization
{

    using Utils.Unity;

    public class LanguageLoader : ExtendedMonoBehaviour
    {
        private void Start()
        {
            this.CoroutineUtils.WaitForFrames(1, () =>
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
