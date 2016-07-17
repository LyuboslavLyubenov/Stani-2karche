using UnityEngine;
using System;

public class StartMenuUIController : MonoBehaviour
{
    public GameObject CreateOrJoinUI;
    public EnterNameUIController EnterNameUIController;
    public HomeScreenTutorialsSwitcher TutorialsSwitcher;

    void Start()
    {
        #if DEBUG
        PlayerPrefs.DeleteKey("Username");
        #endif

        if (CreateOrJoinUI == null)
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
        CreateOrJoinUI.SetActive(true);
    }
}
