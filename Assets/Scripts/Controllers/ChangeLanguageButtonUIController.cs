using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Assets.Scripts.Controllers
{

    using Assets.Scripts.Localization;
    using Assets.Scripts.Utils;

    public class ChangeLanguageButtonUIController : ExtendedMonoBehaviour
    {
        public GameObject AvailableLanguages;

        void Awake()
        {
            this.CoroutineUtils.WaitUntil(this.IsLoadedLanguage, this.OnLoadedLanguage);

            this.GetComponent<Button>().onClick.AddListener(new UnityAction(this.OnMouseDown));
        }

        bool IsLoadedLanguage()
        {
            return LanguagesManager.Instance.IsLoadedLanguage;
        }

        void OnLoadedLanguage()
        {
            this.transform.GetComponentInChildren<Text>().text = LanguagesManager.Instance.Language;
        }

        void OnMouseDown()
        {
            this.AvailableLanguages.SetActive(true);    
        }
    }

}
