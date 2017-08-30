namespace Controllers
{

    using System;

    using EventArgs;

    using Localization;

    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    using Utils.Unity;

    public class ChangeLanguageButtonUIController : ExtendedMonoBehaviour
    {
        public GameObject AvailableLanguages;

        // ReSharper disable once ArrangeTypeMemberModifiers
        void Awake()
        {
            LanguagesManager.Instance.OnLoadedLanguage += OnLoadedLanguage;

            //this.CoroutineUtils.WaitUntil(this.IsLoadedLanguage, this.OnLoadedLanguage);

            this.GetComponent<Button>().onClick.AddListener(new UnityAction(this.OnMouseDown));
        }

        private void OnLoadedLanguage(object sender, LanguageEventArgs args)
        {
            this.transform.GetComponentInChildren<Text>().text = LanguagesManager.Instance.Language;
        }

        private void OnMouseDown()
        {
            this.AvailableLanguages.SetActive(true);    
        }
    }

}
