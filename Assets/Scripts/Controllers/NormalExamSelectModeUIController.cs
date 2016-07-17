using UnityEngine;
using UnityEngine.SceneManagement;

public class NormalExamSelectModeUIController : MonoBehaviour
{
    void Start()
    {
        transform.localScale = new Vector3(1, 0, 1);
    }

    public void OpenHostScene()
    {
        SceneManager.LoadScene("BasicExamHost");    
    }

    public void OpenGuestScene()
    {
        SceneManager.LoadScene("BasicExamGuest");
    }
}
