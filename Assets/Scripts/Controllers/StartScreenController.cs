using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class StartScreenController : MonoBehaviour
{
    public GameObject CreateOrJoinUI;
    public EnterNameUIController EnterNameUIController;

    void Start()
    {
        if (PlayerPrefs.HasKey("Username"))
        {
            EnterNameUIController.gameObject.SetActive(false);
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
