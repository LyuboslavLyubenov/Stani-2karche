using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class GameTypesUIController : MonoBehaviour
{
    void Start()
    {
        transform.localScale = new Vector3(1, 0, 1);
    }

    public void LoadNormalGame()
    {
//TODO:
	throw new NotImplementedException();
        PlayerPrefs.SetString("MainPlayerHost", "true");
        PlayerPrefs.SetString("ServerIP", "127.0.0.1");
        SceneManager.LoadScene("BasicExamMainPlayer", LoadSceneMode.Single);
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
