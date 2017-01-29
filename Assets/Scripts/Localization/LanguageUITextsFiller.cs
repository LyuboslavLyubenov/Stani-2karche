namespace Assets.Scripts.Localization
{

    using Assets.Scripts.EventArgs;

    using UnityEngine.SceneManagement;
    using UnityEngine.UI;

    using Utils.Unity;

    public class LanguageUITextsFiller : ExtendedMonoBehaviour
    {
        // ReSharper disable once ArrangeTypeMemberModifiers
        void Awake()
        {
            DontDestroyOnLoad(this);

            LanguagesManager.Instance.OnLoadedLanguage += this.OnLoadedLanguage;
            SceneManager.sceneLoaded += this.OnSceneLoaded;
        }

        private void OnLoadedLanguage(object sender, LanguageEventArgs args)
        {
            this.TranslateAllTextComponentsInScene();
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            if (LanguagesManager.Instance.IsLoadedLanguage)
            {
                this.CoroutineUtils.WaitForFrames(1, this.TranslateAllTextComponentsInScene);
            }
        }

        private void TranslateAllTextComponentsInScene()
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

}
