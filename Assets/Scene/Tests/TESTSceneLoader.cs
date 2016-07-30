using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.SceneManagement;

public class TESTSceneLoader : MonoBehaviour
{
    public void Load(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
