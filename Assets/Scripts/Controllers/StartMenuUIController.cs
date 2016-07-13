using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class StartMenuUIController : MonoBehaviour
{
    public GameObject GameTypesUI;
    public EnterNameUIController EnterNameUIController;
    public HomeScreenTutorialsSwitcher TutorialsSwitcher;

    void Start()
    {
        if (GameTypesUI == null)
        {
            throw new NullReferenceException("GameTypesUI not found on StartMenuUIController");
        }

        if (EnterNameUIController == null)
        {
            throw new NullReferenceException("EnterNameUIController not found on StartMenuUIController");
        }

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
        TutorialsSwitcher.ExplainGameTypes();
        GameTypesUI.SetActive(true);
    }
}
