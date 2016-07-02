using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class GameTypesUIController : MonoBehaviour
{
    public GameObject NormalExamSelectModeUI;

    void Deactivate()
    {
        GetComponent<Animator>().SetTrigger("disabled");
    }

    public void LoadNormalGame()
    {
        Deactivate();
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
