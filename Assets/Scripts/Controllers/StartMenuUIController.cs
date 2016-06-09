using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class StartMenuUIController : MonoBehaviour
{
    public void LoadGame()
    {
        SceneManager.LoadScene("Playing");
    }
}
