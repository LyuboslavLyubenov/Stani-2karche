namespace Localization
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using EventArgs;

    using UnityEngine.SceneManagement;
    using UnityEngine.UI;

    using Utils.Unity;

    public class LanguageUITextsFiller : ExtendedMonoBehaviour
    {
        //use id as key
        private Dictionary<int, string> uiTextsOldValues = new Dictionary<int, string>();

        // ReSharper disable once ArrangeTypeMemberModifiers
        void Awake()
        {
            DontDestroyOnLoad(this);
            
            LanguagesManager.Instance.OnLoadedLanguage += this.OnLoadedLanguage;
            Assets.Scripts.Utils.SceneManager.OnBeforeLoadScene += this.OnBeforeLoadScene;
            Assets.Scripts.Utils.SceneManager.sceneLoaded += this.OnSceneLoaded;
        }

        private void OnBeforeLoadScene(object sender, EventArgs args)
        {
            this.RestoreTextComponentsPaths();
        }

        private void OnLoadedLanguage(object sender, LanguageEventArgs args)
        {
            this.TranslateUI();
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            if (LanguagesManager.Instance.IsLoadedLanguage)
            {
                this.CoroutineUtils.WaitForFrames(1, this.TranslateUI);
            }
        }

        private Text[] GetAllTextComponentsInScene()
        {
            return GameObjectUtils.GetAllObjectsIncludingInactive()
                .Select(obj => obj.GetComponent<Text>())
                .Where(textComponent => textComponent != null)
                .ToArray();
        }

        private void RestoreTextComponentsPaths()
        {
            if (this.uiTextsOldValues.Count == 0)
            {
                return;
            }

            var allTextComponents = this.GetAllTextComponentsInScene();

            for (int i = 0; i < allTextComponents.Length; i++)
            {
                var textComponent = allTextComponents[i];
                var id = textComponent.gameObject.GetInstanceID();

                if (!this.uiTextsOldValues.ContainsKey(id))
                {
                    continue;
                }

                textComponent.text = this.uiTextsOldValues[id];
            }

            this.uiTextsOldValues.Clear();
        }

        private void TranslateUI()
        {
            this.RestoreTextComponentsPaths();

            var allTextComponents = this.GetAllTextComponentsInScene();

            for (int i = 0; i < allTextComponents.Length; i++)
            {
                var textComponent = allTextComponents[i];
                var id = textComponent.gameObject.GetInstanceID();
                var oldValue = textComponent.text;
                var translation = LanguagesManager.Instance.GetValue(oldValue);

                textComponent.text = translation;
                this.uiTextsOldValues.Add(id, oldValue);
            }
        }
    }
}