using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts.Localization
{

    using Assets.Scripts.Utils;
    using Assets.Scripts.Utils.Unity;

    public class LanguageUITextsFiller : ExtendedMonoBehaviour
    {
        void Awake()
        {
            DontDestroyOnLoad(this);
            LanguagesManager.Instance.OnLoadedLanguage += (sender, args) => this.TranslateAllTextComponentsInScene();
            SceneManager.activeSceneChanged += this.OnSceneChanged;
        }

        void OnSceneChanged(Scene oldScene, Scene newScene)
        {
            this.CoroutineUtils.WaitForFrames(1, this.TranslateAllTextComponentsInScene);
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

}
