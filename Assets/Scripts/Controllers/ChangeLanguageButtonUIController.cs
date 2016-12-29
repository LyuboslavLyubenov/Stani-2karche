namespace Assets.Scripts.Controllers
{
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    using Localization;
    using Utils.Unity;

    public class ChangeLanguageButtonUIController : ExtendedMonoBehaviour
    {
        public GameObject AvailableLanguages;

        // ReSharper disable once ArrangeTypeMemberModifiers
        void Awake()
        {
            this.CoroutineUtils.WaitUntil(this.IsLoadedLanguage, this.OnLoadedLanguage);

            this.GetComponent<Button>().onClick.AddListener(new UnityAction(this.OnMouseDown));
        }

        private bool IsLoadedLanguage()
        {
            return LanguagesManager.Instance.IsLoadedLanguage;
        }

        private void OnLoadedLanguage()
        {
            this.transform.GetComponentInChildren<Text>().text = LanguagesManager.Instance.Language;
        }

        private void OnMouseDown()
        {
            this.AvailableLanguages.SetActive(true);    
        }
    }

}
