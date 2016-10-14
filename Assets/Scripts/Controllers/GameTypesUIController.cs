using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class GameTypesUIController : MonoBehaviour
{
    public GameObject ChooseCategoryUI;

    void Start()
    {
        transform.localScale = new Vector3(1, 0, 1);
    }

    public void LoadNormalGame()
    {
        PlayerPrefsEncryptionUtils.SetString("GameType", "BasicExam");
        ChooseCategoryUI.SetActive(true);
        ChooseCategoryUI.GetComponent<ChooseCategoryUIController>().Initialize();
        gameObject.SetActive(false);
    }

    public void LoadAudienceRevenge()
    {
        //TODO
        throw new NotImplementedException();
    }

    public void LoadFastestWins()
    {
        //TODO
        throw new NotImplementedException();
    }

    public void QuizDuel()
    {
        //TODO
        throw new NotImplementedException();
    }
}
