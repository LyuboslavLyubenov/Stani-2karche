using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ExitButtonController : MonoBehaviour
{
    public void LoadMainScreen()
    {
        SceneManager.LoadScene("StartScreen");        
    }
}
