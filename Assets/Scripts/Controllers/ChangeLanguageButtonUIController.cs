using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ChangeLanguageButtonUIController : ExtendedMonoBehaviour
{
    public GameObject AvailableLanguages;

    void Awake()
    {
        CoroutineUtils.WaitUntil(IsLoadedLanguage, OnLoadedLanguage);

        GetComponent<Button>().onClick.AddListener(new UnityAction(OnMouseDown));
    }

    bool IsLoadedLanguage()
    {
        return LanguagesManager.Instance.IsLoadedLanguage;
    }

    void OnLoadedLanguage()
    {
        transform.GetComponentInChildren<Text>().text = LanguagesManager.Instance.Language;
    }

    void OnMouseDown()
    {
        AvailableLanguages.SetActive(true);    
    }
}
