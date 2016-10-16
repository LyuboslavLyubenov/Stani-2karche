using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public void Load(string name)
    {
        SceneManager.LoadScene(name);
    }
}
