namespace Assets.Scripts.Utils.Unity.UI
{

    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class OpenSceneUI : MonoBehaviour
    {
        Rect windowRect = new Rect(0, 0, 200, 100);

        void OnGUI()
        {
            GUI.ModalWindow(1, this.windowRect, this.OpenSceneRenderer, "OpenScene");
        }

        Rect sceneInputFieldRect = new Rect(5, 10, 140, 50);
        string sceneName = "Type scene name";

        Rect buttonRect = new Rect(5, 60, 140, 30);

        void OpenSceneRenderer(int id)
        {
            this.sceneName = GUI.TextField(this.sceneInputFieldRect, this.sceneName);

            if (GUI.Button(this.buttonRect, "Open scene"))
            {
                SceneManager.LoadScene(this.sceneName);
            }
        }
    }

}
