using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuUIController : MonoBehaviour
{
    public GameObject GameTypesUI;
    public EnterNameUIController EnterNameUIController;

    void Start()
    {
        EnterNameUIController.OnUsernameSet += OnUsernameSet;
    }

    void OnUsernameSet(object sender, System.EventArgs args)
    {
        GameTypesUI.SetActive(true);
    }
}
