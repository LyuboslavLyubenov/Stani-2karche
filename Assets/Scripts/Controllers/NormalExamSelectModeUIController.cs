using UnityEngine;
using UnityEngine.SceneManagement;

public class NormalExamSelectModeUIController : MonoBehaviour
{
    public void OpenHostScene()
    {
        PlayerPrefs.SetString("BasicExamConnectionType", "Player");
        StartGame();
    }

    public void OpenGuestScene()
    {
        PlayerPrefs.SetString("BasicExamConnectionType", "Audience");
        StartGame();
    }

    void StartGame()
    {
        SceneManager.LoadScene("BasicExam");
    }
}
