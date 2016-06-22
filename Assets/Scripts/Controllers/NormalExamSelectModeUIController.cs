using UnityEngine;
using UnityEngine.SceneManagement;

public class NormalExamSelectModeUIController : MonoBehaviour
{
    public void OpenHostScene()
    {
        SceneManager.LoadScene("BasicExamHost");    
    }

    public void OpenGuestScene()
    {
        SceneManager.LoadScene("BasicExamGuest");
    }
}
