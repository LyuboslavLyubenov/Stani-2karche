namespace Utils.Unity.UI
{

    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class OpenSceneUI : MonoBehaviour
    {
        private Rect windowRect = new Rect(0, 0, 200, 100);

        private void OnGUI()
        {
            GUI.ModalWindow(1, this.windowRect, this.OpenSceneRenderer, "OpenScene");
        }

        private Rect sceneInputFieldRect = new Rect(5, 10, 140, 50);

        private string sceneName = "Type scene name";

        private Rect buttonRect = new Rect(5, 60, 140, 30);

        private void OpenSceneRenderer(int id)
        {
            this.sceneName = GUI.TextField(this.sceneInputFieldRect, this.sceneName);

            if (GUI.Button(this.buttonRect, "Open scene"))
            {
                SceneManager.LoadScene(this.sceneName);
            }
        }
    }

}
