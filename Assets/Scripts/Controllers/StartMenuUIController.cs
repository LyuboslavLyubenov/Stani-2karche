using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuUIController : MonoBehaviour
{
    public GameObject GameTypesUI;
    public EnterNameUIController EnterNameUIController;

    void Start()
    {
        if (PlayerPrefs.HasKey("Username"))
        {
            OnUsernameSet(null, System.EventArgs.Empty);
        }
        else
        {
            EnterNameUIController.gameObject.SetActive(true);
            EnterNameUIController.OnUsernameSet += OnUsernameSet;    
        }
    }

    void OnUsernameSet(object sender, System.EventArgs args)
    {
        GameTypesUI.SetActive(true);
    }
}
