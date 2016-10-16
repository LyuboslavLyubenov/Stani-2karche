using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class OpenSceneUI : MonoBehaviour
{
    Rect windowRect = new Rect(0, 0, 200, 100);

    void OnGUI()
    {
        GUI.ModalWindow(1, windowRect, OpenSceneRenderer, "OpenScene");
    }

    Rect sceneInputFieldRect = new Rect(5, 10, 140, 50);
    string sceneName = "Type scene name";

    Rect buttonRect = new Rect(5, 60, 140, 30);

    void OpenSceneRenderer(int id)
    {
        sceneName = GUI.TextField(sceneInputFieldRect, sceneName);

        if (GUI.Button(buttonRect, "Open scene"))
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}
