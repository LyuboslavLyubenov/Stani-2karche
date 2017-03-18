namespace Utils.Unity
{

    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class LoadScene : MonoBehaviour
    {
        public void Load(string name)
        {
            SceneManager.LoadScene(name);
        }
    }

}
