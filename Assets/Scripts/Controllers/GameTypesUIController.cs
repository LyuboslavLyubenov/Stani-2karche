using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class GameTypesUIController : MonoBehaviour
{
    public GameObject NormalExamSelectModeUI;

    public void LoadNormalGame()
    {
        gameObject.SetActive(false);
        NormalExamSelectModeUI.SetActive(true);
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
