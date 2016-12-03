using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ExitButtonController : MonoBehaviour
{
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(LoadMainScreen);
    }

    void LoadMainScreen()
    {
        SceneManager.LoadScene("StartScreen");        
    }
}
