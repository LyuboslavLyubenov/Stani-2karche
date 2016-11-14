using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ChangeLanguageButtonUIController : MonoBehaviour
{
    public GameObject AvailableLanguages;

    void Awake()
    {
        LanguagesManager.Instance.OnLoadedLanguage += (sender, args) =>
        {
            transform.GetComponentInChildren<Text>().text = args.Language;
        };

        GetComponent<Button>().onClick.AddListener(new UnityAction(OnMouseDown));
    }

    void OnMouseDown()
    {
        AvailableLanguages.SetActive(true);    
    }
}
