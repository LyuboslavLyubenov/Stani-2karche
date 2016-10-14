using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class StartScreenController : MonoBehaviour
{
    public GameObject CreateOrJoinUI;
    public EnterNameUIController EnterNameUIController;
    public HomeScreenTutorialsSwitcher TutorialsSwitcher;
    public ChooseCategoryUIController ChooseCategoryUIController;

    void Start()
    {
        ChooseCategoryUIController.OnChoosedCategory += OnChoosedCategory;

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
    //TODO

    void OnChoosedCategory(object sender, ChoosedCategoryEventArgs args)
    {
        var gameType = PlayerPrefsEncryptionUtils.GetString("GameType");

        switch (gameType)
        {
            case "BasicExam":
                OpenBasicExam(args.Name);
                break;
            default:    
                throw new NotImplementedException();
                break;
        }

    }

    void OpenBasicExam(string themeName)
    {
        PlayerPrefsEncryptionUtils.SetString("MainPlayerHost", "true");
        PlayerPrefsEncryptionUtils.SetString("ServerIP", "127.0.0.1");

        PlayerPrefsEncryptionUtils.SetString("ServerLevelCategory", themeName);
        PlayerPrefsEncryptionUtils.SetString("ServerMaxPlayers", "30");

        SceneManager.LoadScene("BasicExamMainPlayer", LoadSceneMode.Single);    
    }
}
