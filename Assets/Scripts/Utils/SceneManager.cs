namespace Assets.Scripts.Utils
{

    using System;

    using UnityEngine.Internal;
    using UnityEngine.SceneManagement;

    public class SceneManager : UnityEngine.SceneManagement.SceneManager
    {
        public static event EventHandler OnBeforeLoadScene = delegate { };

        public new static void LoadScene(string sceneName)
        {
            OnBeforeLoadScene(null, EventArgs.Empty);
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        }

        public new static void LoadScene(string sceneName, [DefaultValue("LoadSceneMode.Single")] LoadSceneMode mode)
        {
            OnBeforeLoadScene(null, EventArgs.Empty);
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName, mode);
        }

        public new static void LoadScene(int sceneBuildIndex)
        {
            OnBeforeLoadScene(null, EventArgs.Empty);
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneBuildIndex);
        }

        public new static void LoadScene(int sceneBuildIndex, [DefaultValue("LoadSceneMode.Single")] LoadSceneMode mode)
        {
            OnBeforeLoadScene(null, EventArgs.Empty);
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneBuildIndex, mode);
        }
    }
}
